import { motion } from "motion/react"
import { useEffect, useState } from "react"
import { getAqiFiguresByUIDs } from "../Api/ApiClient"
import type { AirQualityDataSetDto } from "../Api/ApiClient"

interface CityData {
  city: string;
  country: string;
  aqi: number;
  pollutant: string;
  stationId: string;
}

// Static city info with station IDs
const cityStations = [
  { city: "New York", country: "USA", stationId: "3307" },
  { city: "London", country: "UK", stationId: "5724" },
  { city: "Tokyo", country: "Japan", stationId: "2302" },
  { city: "Beijing", country: "China", stationId: "1451" },
  { city: "Paris", country: "France", stationId: "5722" },
  { city: "Sydney", country: "Australia", stationId: "12417" },
  { city: "Mexico City", country: "Mexico", stationId: "404" },
  { city: "São Paulo", country: "Brazil", stationId: "359" },
  { city: "Cape Town", country: "South Africa", stationId: "12829" },
  { city: "Mumbai", country: "India", stationId: "12454" },
  { city: "Los Angeles", country: "USA", stationId: "A399061" },
  { city: "Lagos", country: "Nigeria", stationId: "A546313" },
  { city: "Moscow", country: "Russia", stationId: "10486" }
];

const getAQIColor = (aqi: number) => {
  if (aqi <= 50) return "text-green-600";
  if (aqi <= 100) return "text-yellow-600";
  if (aqi <= 150) return "text-orange-600";
  if (aqi <= 200) return "text-red-600";
  if (aqi <= 300) return "text-purple-600";
  return "text-red-800";
};

const getAQIBg = (aqi: number) => {
  if (aqi <= 50) return "bg-green-100";
  if (aqi <= 100) return "bg-yellow-100";
  if (aqi <= 150) return "bg-orange-100";
  if (aqi <= 200) return "bg-red-100";
  if (aqi <= 300) return "bg-purple-100";
  return "bg-red-200";
};



export function TickerTape() {
  const [cityData, setCityData] = useState<CityData[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const fetchCityData = async () => {
      try {
        // Extract all station IDs
        const stationIds = cityStations.map(station => station.stationId);
        
        // Call the batch API endpoint
        const apiData: Record<string, AirQualityDataSetDto> = await getAqiFiguresByUIDs(stationIds);
        
        // Map the API response to city data
        const updatedCityData = cityStations.map(station => {
          const stationData = apiData[station.stationId];
          
          return {
            city: station.city,
            country: station.country,
            stationId: station.stationId,
            aqi: stationData?.data?.aqi ?? 0,
            pollutant: stationData?.data?.dominentpol?.toUpperCase() ?? "N/A"
          };
        });
        
        setCityData(updatedCityData);
        setIsLoading(false);
      } catch (error) {
        console.error("Error fetching ticker tape data:", error);
        setIsLoading(false);
      }
    };

    fetchCityData();
    
    // Refresh data every 10 minutes
    const interval = setInterval(fetchCityData, 600000);
    
    return () => clearInterval(interval);
  }, []);

  if (isLoading) {
    return (
      <div className="w-full bg-muted border-t overflow-hidden py-1 lg:py-2 fixed bottom-0">
        <div className="flex justify-center items-center h-8">
          <span className="text-xs text-muted-foreground">Loading air quality data...</span>
        </div>
      </div>
    );
  }

  const duplicatedData = [...cityData, ...cityData];

  return (
    <div className="w-full bg-muted border-t overflow-hidden py-1 lg:py-2 fixed bottom-0">
        <motion.div
          className="flex gap-8 whitespace-nowrap"
          animate={{
            x: [0, -50 * duplicatedData.length],
          }}
          transition={{
            duration: 60,
            repeat: Infinity,
            ease: "linear",
          }}>
          {duplicatedData.map((city, index) => (
            <div
              key={`${city.city}-${index}`}
              className="flex items-center gap-3 px-4 py-0.3 rounded-full bg-background shadow-sm min-w-fit">
              <span className="text-xs lg:text-sm font-medium">
                {city.city}, {city.country}
              </span>
              <span
                className={`text-tiny lg:text-xs px-2 py-0.1 lg:py-0.3 rounded-full ${getAQIBg(
                  city.aqi
                )} ${getAQIColor(city.aqi)} font-medium`}>
                AQI {city.aqi}
              </span>
              <span className="text-tiny lg:text-xs text-muted-foreground">
                {city.pollutant}
              </span>
            </div>
          ))}
        </motion.div>
      </div>

  );
}