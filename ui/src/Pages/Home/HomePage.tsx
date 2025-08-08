import { useState } from "react";
import { TickerTape } from "../.././components/TickerTape";
import AqiFiguresDisplay from "../.././components/AqiFiguresDisplay";
import type { AirQualityDataSetDto } from "../.././Api/ApiClient";
import { FindDataForNearestStationForm, type LongLat } from "../.././components/FormComponents/FindDataForNearestStationForm";
import "leaflet/dist/leaflet.css";
import "../.././styles/globals.css";
import "../.././styles/app.css";

const HomePage = () => {

  const [currentLongLat, setCurrentLongLat] = useState<LongLat>({Longitude: 51.5072,Latitude: 0.1276});    
    const [aqiForClosestStation, setAqiForClosestStation] = useState<AirQualityDataSetDto | null>(null);
    return (
        
             <>
      <div className="min-h-95vh flex flex-col min-w-screen items-center mt-50 rounded-sm">
        <div className="flex">
          <AqiFiguresDisplay currentLongLat={currentLongLat} aqiForClosestStation={aqiForClosestStation} onAqiChange={setAqiForClosestStation}/>
        </div>
        <FindDataForNearestStationForm currentLongLat={currentLongLat} onCoordinatesChange={setCurrentLongLat} />
      </div>
      <TickerTape />
    </>

        
        );
}
export default HomePage;