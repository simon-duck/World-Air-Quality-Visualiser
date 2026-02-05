<h1 align="center">Welcome to WorldAirQualityVisualiser 👋</h1>
<p>
</p>

> World Air Quality Visualiser is a full-stack web app for exploring location based live air quality data through 3D visualiations driven by React Three Fiber. Air quality data is provided by the API provided at https://aqicn.org/

### 🏠 [Homepage](https://worldairqualityvisualiser.online/)


## Install

### Prerequisites

- [Node.js](https://nodejs.org/) and npm
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

1. **Clone the repository**
   ```sh
   git clone https://github.com/the-duckchild/Air-Pollution-Visualiser.git
   cd Air-Pollution-Visualiser
   ```

2. **Install frontend dependencies**
   ```sh
   cd ui
   npm install
   ```

3. **Install backend dependencies**
   ```sh
   cd ../api
   dotnet restore
   ```
4. **Configure environment variables**
    - Create .env file in both `ui/` and `api/` folders.
   - Edit the 'ui' .env file to  include - `VITE_API_BASE_URL`: set to your backend API endpoint.
   - Edit the 'api' .env file to include = `API_EXTERNAL_KEY`: set your API key from[https://aqicn.org/data-platform/token/].
   - For detailed setup, see comments in each `.env.example`

5. **Run the backend**
   ```sh
   cd api
   dotnet run
   ```

6. **Run the frontend**
   ```sh
   cd ../ui
   npm run dev
   ```

## Run tests

8. **Run the backend tests**
   ```sh
   cd api.tests
   dotnet test
   ```

6. **Run the frontend tests**
   ```sh
   cd ../ui
   npm run dev
   ```

## Usage

- Visit [http://localhost:5173](http://localhost:5173) in your browser.
- Interact with the world map to:
  - Select a region and locate the nearest air pollution monitoring station.
  - View current Air Quality Index figures and see animated 3D visualisations.
  - Watch the live ticker for updates as data changes worldwide.
  - Enjoy the realistic Sun position for the current local time.

## Troubleshooting

- **.env Issues**: Ensure your `.env` files are correctly copied and contain all required variables and API keys.
- **Port Conflicts**: The frontend defaults to port 5173, backend typically to 5000 or 8000. Close any apps using these ports or adjust in your config.
- **CORS Errors**: Double-check your backend CORS configuration (`api/appsettings.json` or environment variables) to allow requests from your frontend's origin.
- **API Key/Quota Issues**: If you see “API Limit Exceeded” errors, confirm your external key is valid and not over its usage quota.
- **.NET Build Errors**: Verify you’ve installed the correct version of the .NET SDK.

## Configuration

- **Frontend (`ui/.env`)**:  
  - Set the backend API URL (`VITE_API_BASE_URL`), map options, and UI features.
- **Backend (`api/.env`, `api/appsettings.json`)**:  
  - Configure database connection, CORS, and external air quality API keys.
- **Styling**:  
  - Edit `ui/src/styles/app.css` and `ui/src/styles/globals.css` for custom designs.
- **Map & Visuals**:  
  - Tweak defaults and behaviour in `ui/src/Pages/Home/HomePage.tsx` and `ui/src/components/AqiVisualiser/AqiVisualiser.tsx`.

## Structure

1. **Backend API**

- **Program**:
    - The [program](https://github.com/simon-duck/World-Air-Quality-Visualiser/blob/main/api/Program.cs) bootstraps the API, ensuring all necessary components are initialized and the application is ready to handle HTTP requests.

- **Repository**:
    - The [repository](https://github.com/simon-duck/World-Air-Quality-Visualiser/blob/main/api/Repositories/AirQualityDataRepository.cs) contains the logic to interact with the API at [aqicn.org](https://aqicn.org/api/)

- **Controller**: 
    - The [controller](https://github.com/simon-duck/World-Air-Quality-Visualiser/blob/main/api/Controllers/AirQualityDataController.cs) defines the endpoints to access AQI data by UID(s) or Latitude/Longitude. 

- **Sanitization**:
    - The controller calls the [sanitization](https://github.com/simon-duck/World-Air-Quality-Visualiser/blob/main/api/Services/InputSanitizationService.cs) service to clean and validate user input to prevent invalid data or injection attacks. 


2. **Data Models**

    - The DTO is defined in the backend [here](https://github.com/simon-duck/World-Air-Quality-Visualiser/blob/main/api/Models/Dto/AirQualityDataSetDto.cs)
    - The DTO is defined in the frontend [here](https://github.com/simon-duck/World-Air-Quality-Visualiser/blob/main/ui/src/Api/ApiClient.tsx) which also contains the logic for sending requests to the backend API.


3. **Frontend UI**

    

      - Routing takes place from [App](https://github.com/simon-duck/World-Air-Quality-Visualiser/blob/main/ui/src/App.tsx)       
  
      - This displays the [Home](https://github.com/simon-duck/World-Air-Quality-Visualiser/blob/main/ui/src/Pages/Home/HomePage.tsx) page, which currently manages a lot of the state for the app.
    
    - **Form Components**:

      - (FindDataForNearestStationForm.tsx)[https://github.com/simon-duck/World-Air-Quality-Visualiser/blob/main/ui/src/components/FormComponents/FindDataForNearestStationForm.tsx] displays the context dependent "hide map" and "show map" button and allows the submission of long/lat data by a map click.

      - [MapComponent.tsx](https://github.com/simon-duck/World-Air-Quality-Visualiser/blob/main/ui/src/components/FormComponents/MapComponent.tsx) manages the map visibility, initial coordinates and user interactions on the map, such as panning and zooming.
    
      - [LocationMapMarker](https://github.com/simon-duck/World-Air-Quality-Visualiser/blob/main/ui/src/components/FormComponents/LocationMarkerMap.tsx) displays the map marker and updates both it's position and lat/long form fields on mouse click.

    - **AQIVisualiser**:
        
      - [AqiVisualiser.tsx](https://github.com/simon-duck/World-Air-Quality-Visualiser/blob/main/ui/src/components/AqiVisualiser/AqiVisualiser.tsx) is the main 3D visualisation using [React Three Fiber](https://r3f.docs.pmnd.rs/getting-started/introduction) to show particle systems with the number of particles based on the current values of air pollutants at the nearest recording station to the selected location. 
  
      - [Particle Systems](https://github.com/simon-duck/World-Air-Quality-Visualiser/blob/main/ui/src/components/AqiVisualiser/ParticleSystems.tsx) defines the individual particle systems and was inspired by this ThreeJs [example](https://threejs.org/examples/#webgl_buffergeometry_drawrange), the code for which is [here](https://github.com/mrdoob/three.js/blob/master/examples/webgl_buffergeometry_drawrange.html). It also handles collision detection for both other particles and the bounds of the containing box. 

      - [Clouds](https://github.com/simon-duck/World-Air-Quality-Visualiser/blob/main/ui/src/components/AqiVisualiser/Clouds.tsx) and [Sun](https://github.com/simon-duck/World-Air-Quality-Visualiser/blob/main/ui/src/components/AqiVisualiser/Sun.tsx) use elements from the [React Three Drei](https://drei.docs.pmnd.rs/getting-started/introduction) library to provide a background for the visualisation. The Sun visualisation reacts to the current local timezone, using the browser-geo-tz and date-fns-tz libraries.

      - [Grass] provides the ground for the visualisation, and is based on the codesandbox [here](https://codepen.io/al-ro/pen/jJJygQ)

    - **AQI Figures Displays**:
  
      - [AQIFiguresDisplay](https://github.com/simon-duck/World-Air-Quality-Visualiser/blob/main/ui/src/components/AqiFiguresDisplay.css) displays the responsive control panel with the current AQI values for the selected location, the location and local time and controls to activate or deactivate the visualisation for each available pollutant.

      - [TickerTape](https://github.com/simon-duck/World-Air-Quality-Visualiser/blob/main/ui/src/components/TickerTape.tsx) displays a moving ticker-tape display at the bottom of the screen with current AQI values for major world cities. It uses [motion/react](https://motion.dev/docs/react) to create the scroll and updates the live data every ten minutes.

## Author

👤 **Simon Duck**

* Github: [@simon-duck](https://github.com/simon-duck)
* LinkedIn: [@simon-duck](https://linkedin.com/in/simon-duck)

## Show your support

Give a ⭐️ if this project helped you!

***
_This README was generated with ❤️ by [readme-md-generator](https://github.com/kefranabg/readme-md-generator)_
