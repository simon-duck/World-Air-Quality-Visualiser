import React, { useEffect } from "react";
import { getAqiFiguresByLatLon } from "../Api/ApiClient";
import type { AirQualityDataSetDto } from "../Api/ApiClient";
import "./AqiFiguresDisplay.css";
import { type LongLat } from "../components/FormComponents/FindDataForNearestStationForm";

interface AqiFiguresDisplayProps {
  currentLongLat: LongLat;
  aqiForClosestStation: AirQualityDataSetDto | null;
  onAqiChange: (coordinates: AirQualityDataSetDto) => void;
}

const AqiFigures: React.FC<AqiFiguresDisplayProps> = ({ 
  currentLongLat, 
  aqiForClosestStation,
  onAqiChange 
}) => {
 
console.log(currentLongLat)

  useEffect(() => {
    const fetchAqiData = async () => {
      try {
        const data = await getAqiFiguresByLatLon(currentLongLat.Latitude, currentLongLat.Longitude);
        onAqiChange(data);
      } catch (error) {
        console.error("Error fetching AQI data:", error);
      }
    };

   fetchAqiData();
  }, [currentLongLat.Latitude, currentLongLat.Longitude, onAqiChange]);

  return (
    <div className="aqi bg-white p-4 flex flex-col w-200 rounded-md space-y-2 m-5">
      <div className="flex flex-row">
      <h3 className="font-bold text-lg mb-2">Air Quality Data</h3> 
      <p><strong>Location:</strong> {aqiForClosestStation?.data?.city?.name || 'Unknown'}</p>
      </div>
      <div className="grid grid-cols-4 gap-4">
        <div className="">
          <p><strong>Overall AQI:</strong> {aqiForClosestStation?.data?.aqi || 'N/A'}</p>
          

        </div>
        <div>
          <p><strong>PM10:</strong> {aqiForClosestStation?.data?.iaqi?.pm10?.v || 'N/A'} μg/m³</p>
          <p><strong>PM2.5:</strong> {aqiForClosestStation?.data?.iaqi?.pm25?.v || 'N/A'} μg/m³</p>                 
          
          
        </div>
        <div>
          <p><strong>CO₂:</strong> {aqiForClosestStation?.data?.iaqi?.co2?.v || 'N/A'} μg/m³</p>
          <p><strong>CO:</strong> {aqiForClosestStation?.data?.iaqi?.co?.v || 'N/A'} μg/m³</p>
        </div>
        <div>
          <p><strong>NO₂:</strong> {aqiForClosestStation?.data?.iaqi?.no2?.v || 'N/A'} μg/m³</p>
          <p><strong>SO₂:</strong> {aqiForClosestStation?.data?.iaqi?.so2?.v || 'N/A'} μg/m³</p>  
        </div>
      </div>
      <div className="mt-2">
        
        
      </div>
    </div>
  );
};

export default AqiFigures;
