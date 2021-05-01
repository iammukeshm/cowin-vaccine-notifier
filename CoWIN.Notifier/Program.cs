using CoWIN.Notifier.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CoWIN.Notifier
{
    class Program
    {
        public static CoWINConfiguration cowinConfiguration;
        static void Main(string[] args)
        {
            IConfiguration Configuration = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables()
                        .AddCommandLine(args)
                        .Build();
            cowinConfiguration = new CoWINConfiguration();
            Configuration.GetSection("CoWINConfiguration").Bind(cowinConfiguration);
            var intervalInMinutes = cowinConfiguration.IntervalInMinutes < 5 ? 5 : cowinConfiguration.IntervalInMinutes;
            Timer timer = new Timer(StartCoWINNotifier, null, 0, intervalInMinutes * 60000);
            Console.WriteLine("Starting CoWIN Notifier. Press Enter to Exit.");
            Console.ReadLine();
        }
        public async static void StartCoWINNotifier(object args)
        {
            Console.Clear();
            Console.WriteLine($"Pinging CoWIN API - {DateTime.Now}");
            var dates = GetNextTwoDays();
            var cowinResponses = new List<CoWINResponse>();
            foreach (var date in dates)
            {
                var data = await CoWINService.Ping(cowinConfiguration.DistrictId, date);
                if (data == null) continue;
                if (data.centers.Count == 0) continue;
                cowinResponses.Add(data);
            }
            var refinedData = new List<AvailableCenter>();
            foreach (var response in cowinResponses)
            {
                foreach (var center in response.centers)
                {
                    foreach (var session in center.sessions)
                    {
                        if (session.available_capacity > 0)
                        {
                            refinedData.Add(new AvailableCenter
                            {
                                Id = center.center_id,
                                AvailableVaccines = session.available_capacity,
                                DateTime = DateTime.Parse(session.date),
                                District = center.district_name,
                                MinimumAge = session.min_age_limit,
                                Name = center.name,
                                State = center.state_name
                            });
                        }
                    }
                }
            }
            var distictData = refinedData.GroupBy(x => new { x.Id, x.DateTime, x.AvailableVaccines, x.District, x.MinimumAge, x.Name, x.State }).Select(y => new AvailableCenter()
            {
                AvailableVaccines = y.Key.AvailableVaccines,
                District = y.Key.District,
                State = y.Key.State,
                Name = y.Key.Name,
                MinimumAge = y.Key.MinimumAge,
                DateTime = y.Key.DateTime,
                Id = y.Key.Id
            });
            var dataDump = string.Empty;
            var actualData = distictData.GroupBy(a => a.Id);
            foreach (var data in distictData.OrderBy(a => a.DateTime).Distinct().ToList())
            {
                dataDump += $"{data.AvailableVaccines} vaccines available at {data.Name}, {data.District} , { data.State} on {data.DateTime.ToString("dd/MM/yyyy")}." + Environment.NewLine;
            }
            if(!string.IsNullOrEmpty(dataDump))
            {
                await CoWINService.Notify($"Vaccinations Available at {distictData.GroupBy(a=>a.Id).Count()} centres in {distictData.Select(a=>a.District).FirstOrDefault()}.",dataDump, cowinConfiguration.IFTTT_ApiKey);
            }
            Console.WriteLine(dataDump.Trim());
        }
        private static List<DateTime> GetNextTwoDays()
        {
            var dates = new List<DateTime>();
            var today = DateTime.Now.Date;
            for (var dt = today; dt <= today.AddDays(2); dt = dt.AddDays(1))
            {
                dates.Add(dt);
            }
            return dates;
        }
    }
}
