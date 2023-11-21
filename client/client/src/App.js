import "./App.css";
import React, { useEffect, useState } from "react";
import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import Home from "./pages/Home";
import Navbar from "./components/Navbar";
import Footer from "./components/Footer";
import Filter from "./pages/Filter";
import Graphs from "./pages/Graphs";
import About from "./pages/About";
import Contact from "./pages/Contact";
import ProvidersRank from "./pages/ProvidersRank";
import DataContext from "./DataContext";

function App() {
  const [percentageCPU, setPercentageCPU] = useState([]);
  const [percentageMemory, setpercentageMemory] = useState([]);
  const [incomingTraffic, setincomingTraffic] = useState([]);
  const [outcomingTraffic, setoutcomingTraffic] = useState([]);

  const fetchData = async () => {
    let url = `http://localhost:8496/MetricsResults/GetMetrics`;
    try {
      const response = await fetch(url);
      const data = await response.json();
      setPercentageCPU(data.PercentageCPU.split(","));
      setpercentageMemory(data.PercentageMemory.split(","));
      setincomingTraffic(data.IncomingTraffic.split(","));
      setoutcomingTraffic(data.OutcomingTraffic.split(","));
      console.log(percentageCPU);
    } catch (error) {
      console.error(`Failed to fetch data`, error);
    }
  };

  useEffect(() => {
    fetchData();

    const intervalId = setInterval(fetchData, 3600000);
    return () => {
      clearInterval(intervalId);
    };
  }, []);

  return (
    <div className="App">
      <div className="content">
        <Router>
          <Navbar />
          <DataContext.Provider
            value={{
              percentageCPU,
              percentageMemory,
              incomingTraffic,
              outcomingTraffic,
            }}
          >
            <Routes>
              <Route path="/" element={<Home />} />
              <Route path="/filter" element={<Filter />} />
              <Route path="/graphs" element={<Graphs />} />
              <Route path="/providersRank" element={<ProvidersRank />} />
              <Route path="/about" element={<About />} />
              <Route path="/contact" element={<Contact />} />
            </Routes>
          </DataContext.Provider>
        </Router>
      </div>
      <Footer />
    </div>
  );
}

export default App;
