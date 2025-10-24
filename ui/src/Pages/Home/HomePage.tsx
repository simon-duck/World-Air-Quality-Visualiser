import { useState } from "react";

import { TickerTape } from "../.././components/TickerTape";
import AqiFiguresDisplay from "../.././components/AqiFiguresDisplay";
import {AqiVisualiser} from "../../components/AqiVisualiser/AqiVisualiser";
import type { AirQualityDataSetDto, Iaqi } from "../.././Api/ApiClient";
import { FindDataForNearestStationForm, type LongLat } from "../.././components/FormComponents/FindDataForNearestStationForm";
import "leaflet/dist/leaflet.css";
import "../.././styles/globals.css";
import "../.././styles/app.css";

const HomePage = () => {
  const [enabledSystems, setEnabledSystems] = useState<Record<string, boolean>>({
    co: true,
    co2: true,
    no2: true,
    pm10: true,
    pm25: true,
    so2: true,
  });

  // const handleToggleSystem = (key: string) => {
  //   setEnabledSystems((prev) => ({
  //     ...prev,
  //     [key]: !prev[key],
  //   }));
  // };

  // Fallback iaqi data with zero values
  const fallbackIaqi: Iaqi = {
    co: { v: 0 },
    co2: { v: 0 },
    no2: { v: 0 },
    pm10: { v: 0 },
    pm25: { v: 0 },
    so2: { v: 0 },
  };

  const [currentLongLat, setCurrentLongLat] = useState<LongLat>({Longitude: -0.1276, Latitude: 51.5072});    
    const [aqiForClosestStation, setAqiForClosestStation] = useState<AirQualityDataSetDto | null>(null);
    return (
        
             <>
             
             <div className = "flex justify-between w-screen">
               <img src="High-Resolution-Color-Logo-on-Transparent-Background_edited.png" className="object-scale-down h-30"></img>

          <AqiFiguresDisplay currentLongLat={currentLongLat} aqiForClosestStation={aqiForClosestStation} onAqiChange={setAqiForClosestStation}/>
          
        </div>
      <div className="min-h-95vh flex flex-col min-w-screen items-center">
        
        
          <AqiVisualiser 
            data={aqiForClosestStation?.data?.iaqi || fallbackIaqi} 
            enabledSystems={enabledSystems}
            longitude={currentLongLat.Longitude}
            latitude={currentLongLat.Latitude}
          />

        
      
        <FindDataForNearestStationForm currentLongLat={currentLongLat} onCoordinatesChange={setCurrentLongLat} />
      </div>
      <TickerTape />
    </>

        
        );
}
export default HomePage;