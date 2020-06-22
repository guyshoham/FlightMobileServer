# FlightMobileServer

![alt text](https://github.com/TomSendrovich/FlightMoblieApp/blob/master/screenshots/Portrait.png)
![alt text](https://github.com/TomSendrovich/FlightMoblieApp/blob/master/screenshots/Landscape.png)

An Android app that controls a plane through the Simulator Flight.  
The app communicates using an HTTP protocol to a proxy server, which will send our requests to the simulator.  
The proxy server can connect to only one simulator, so it can only serve one client.

The app send control commands to the simulator, and also receive screenshot images and display it on screen  

### Objectives:
* Using **ASP.NET Core 3.1** to create **REST API** using WebAPI. :white_check_mark:
* 100% Kotlin :white_check_mark:
* Database using **room** library :white_check_mark:
* HTTP request using **retrofit** library :white_check_mark:
* I/O requests using **Kotlin Coroutines** :white_check_mark:

### Installation

1. Clone both [App](https://github.com/TomSendrovich/FlightMoblieApp) and [Server](https://github.com/guyshoham/FlightMobileServer) projects to your machine.
2. Create **Pixel XL** emulator in Android Studio
3. Download and install [FlightGear](https://www.flightgear.org/download/)

## Set FlightGear Simulator

1. Open FlightGear Simulator.
2. Click Settings
3. Scroll down to Additional Settings
4. type the following commands:

```bash
--telnet=socket,in,10,127.0.0.1,5402,tcp
--httpd=8080
```

### Build And Run

1. Run FlightGear Simulator
2. Run the server
3. Run the application
