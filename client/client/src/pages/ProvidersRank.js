import React, { useContext } from "react";
import "../styles/ProvidersRank.css";
import BannerImage from "../assets/graphsBack.png";
import DataContext from "../DataContext";
import one from "../assets/one.png";
import two from "../assets/two.png";
import three from "../assets/three.png";

const ProvidersRank = () => {
  const { percentageCPU, percentageMemory, incomingTraffic, outcomingTraffic } =
    useContext(DataContext);

  const renderRankCard = (title, data, images) => (
    <div className="rank-card">
      <h2>{title}</h2>
      <ol>
        {data.map((item, index) => (
          <li key={index}>
            <div className="rank-position">
              {index === 0 && <img src={images[0]} alt="1" />}
              {index === 1 && <img src={images[1]} alt="2" />}
              {index === 2 && <img src={images[2]} alt="3" />}
            </div>
            {item}
          </li>
        ))}
      </ol>
    </div>
  );

  return (
    <div
      className="providersRank"
      style={{ backgroundImage: `url(${BannerImage})` }}
    >
      <h1>Providers Rank</h1>
      <p>
        Welcome to the Providers Rank page! Here, you can discover the rankings
        of the providers across CPU performance, memory utilization, incoming
        traffic and outcoming traffic. Our rankings are regularly updated every
        hour, ensuring you have the latest insights into the performance of
        different providers. Explore the rankings for each criterion and
        identify the top performers in each category. <br />
        We hope these rankings empower you to make informed decisions that align
        with your unique requirements.
      </p>
      <div className="rank-container">
        {renderRankCard("CPU Performance", percentageCPU, [one, two, three])}
        {renderRankCard("Memory Utilization", percentageMemory, [
          one,
          two,
          three,
        ])}
        {renderRankCard("Incoming Traffic", incomingTraffic, [one, two, three])}
        {renderRankCard("Outgoing Traffic", outcomingTraffic, [
          one,
          two,
          three,
        ])}
      </div>
    </div>
  );
};

export default ProvidersRank;
