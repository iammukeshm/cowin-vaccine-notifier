# COWIN VACCINE NOTIFIER

A Small POC Project that sends out a notification message on your Mobile Devices whenever there are vaccinations available in your District. In-Depth Details are also available as a part of the notification message via the IFTTT application. Written in C# .NET 5.0.
 
 Here are a few screenshots of the notifications you would be receiving.
 
 ![Event](https://codewithmukesh.com/wp-content/uploads/2021/05/WhatsApp-Image-2021-05-01-at-10.45.53-PM.jpeg)
  ![Event](https://codewithmukesh.com/wp-content/uploads/2021/05/WhatsApp-Image-2021-05-01-at-10.43.52-PM-1.jpeg)
  
   ![Event](https://codewithmukesh.com/wp-content/uploads/2021/05/VsDebugConsole_6hIwtsC33K.jpg)
## Setting up the Application

 - Install the IFTTT Application on to your mobile device.
 - Configure IFTTT as mentioned in this [article](https://betterprogramming.pub/how-to-send-push-notifications-to-your-phone-from-any-script-6b70e34748f6).
 - Set the default trigger name of your new IFTTT applet as `vaccination-available`

Quick Video of the setup - https://user-images.githubusercontent.com/31455818/116820335-1e7cc380-ab92-11eb-9d8d-a5ef5a29f2ba.mp4
 
 ![Event](https://codewithmukesh.com/wp-content/uploads/2021/05/chrome_MnXjYdBR14.jpg)
 - Replace `IFTTT_ApiKey`value in the `appsettings.json` file of the Console project to your API Key. 
 
 You can find your apiKey by clicking on your IFTTT profile picture -> My Services  ->  Webhooks -> Documentation . Here you will be able to see your applet's API Key.
 - Replace `IntervalInMinutes` and `DistrictId`as well with the required data.
 
 ### District Codes
 - Trivandrum - 296
 - Kollam - 298
 - Chennai - 571
 - Kanyakumari - 574
 - Central Delhi - 141
 - Bangalore Urban - 265
 - Bangalore Rural - 276
 
 > As of now, I am only adding in a few of the Districts. You can get the required district by a simple hack. 
 
 - Navigate to CoWIN Portal.
 - Open Developer Tools -> Network Tab
 - Select the Required State from the UI.
 - Check the Data in the Network Tab. Screenshot attached below
 
 ![Getting Districts](https://codewithmukesh.com/wp-content/uploads/2021/05/chrome_NvLvLQRucm.jpg)
 
 
# IMPORTANT

Please do not abuse this API endpoint as it is something very critical. Avoid sending requests to the API Endpoint every few minutes. I recommend an interval of 20 minutes ideally.
