import React from "react";
import { Link } from "react-router-dom";
import "../styles/Home.css";
import BannerImage from "../assets/background.png";

function Home() {
  return (
    <div className="home" style={{ backgroundImage: `url(${BannerImage})` }}>
      <div className="headerContainer">
        <h1> Welcome to Cloud Fit </h1>
        <p> Here, you will find your best cloud match!</p>
        <Link to="/filter">
          <button> GET STARTED </button>
        </Link>
      </div>
    </div>
  );
}

export default Home;
