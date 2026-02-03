<h1 align="center">Welcome to WorldAirQualityVisualiser 👋</h1>
<p>
</p>

> World Air Quality Visualiser is a full-stack web app for exploring location based live air quality data through 3D visualisations driven by React Three Fiber. Air quality data is provided by the API provided at [https://aqicn.org/]

### 🏠 [Homepage](https://worldairqualityvisualiser.online/)

[![Netlify Status](https://api.netlify.com/api/v1/badges/ef6f6594-1863-40b2-b2b9-1cfb9025f7a5/deploy-status)](https://app.netlify.com/projects/worldairqualityvisualiser/deploys)

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
   - Edit the 'ui' .env file to  include `VITE_API_BASE_URL`: set to your backend API endpoint.
   - Edit the 'api' .env file to include `API_EXTERNAL_KEY`: set your API key from [https://aqicn.org/data-platform/token/].
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

7. **Run the frontend tests**
   ```sh
   cd ../ui
   npm run dev
   ```
   
8. **Run the backend tests**
   ```sh
   cd api.tests
   dotnet test
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

## Author

👤 **Simon Duck**

* Github: [@simon-duck](https://github.com/simon-duck)
* LinkedIn: [@simon-duck](https://linkedin.com/in/simon-duck)

## Show your support

Give a ⭐️ if this project helped you!

***
_This README was generated with ❤️ by [readme-md-generator](https://github.com/kefranabg/readme-md-generator)_
