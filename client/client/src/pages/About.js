import React from "react";
import AboutPhoto from "../assets/aboutPhoto.jpg";
import "../styles/About.css";

function About() {
  return (
    <div className="about">
      <div
        className="aboutTop"
        style={{ backgroundImage: `url(${AboutPhoto})` }}
      ></div>
      <div className="aboutBottom">
        <h1> ABOUT US</h1>
        <p>
          Our application empowers you to conduct stress tests on virtual
          machines with identical CPU and memory properties. With just a few
          clicks, you can choose your preferred cloud provider, specify desired
          machine properties, and even select your preferred location. It's all
          about customization! But we don't stop there. Our application goes the
          extra mile by providing access to a comprehensive virtual machine
          history based on your individual preferences. You can track
          performance metrics, analyze trends, and make data-driven decisions.
        </p>
      </div>
    </div>
  );
}

export default About;
