import React, { useState } from "react";
import { Select, MenuItem, Button } from "@mui/material";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";
import "../styles/TimeSelection.css";

const TimeSelection = ({
  onSelectChange,
  onDateChange,
  isRealTime,
  isCustom,
}) => {
  const [startDate, setStartDate] = useState("");
  const [endDate, setEndDate] = useState("");
  const [selectedValue, setSelectedValue] = useState("Real-time");
  const [isButtonDisabled, setIsButtonDisabled] = useState(true);

  const handleSelectChange = (event) => {
    const value = event.target.value;
    setSelectedValue(value);
    onSelectChange(value);

    if (value === "Custom" || value === "Real-time") {
      setStartDate("");
      setEndDate("");
      setIsButtonDisabled(true);
    }
    if (value === "Last-Week") {
      const now = new Date();
      const oneWeekAgo = new Date(now.getTime() - 7 * 24 * 60 * 60 * 1000);
      setStartDate(now.toISOString().split(".")[0] + "Z");
      setEndDate(oneWeekAgo.toISOString().split(".")[0] + "Z");
      onDateChange(oneWeekAgo, now);
    } else if (value === "Last-Month") {
      const now = new Date();
      const oneMonthAgo = new Date(now);
      oneMonthAgo.setMonth(oneMonthAgo.getMonth() - 1);
      setStartDate(now.toISOString().split(".")[0] + "Z");
      setEndDate(oneMonthAgo.toISOString().split(".")[0] + "Z");
      onDateChange(oneMonthAgo, now);
    }
  };

  const handleDateChange = (date, type) => {
    if (type === "start") {
      setStartDate(date);
    } else if (type === "end") {
      setEndDate(date);
    }
    console.log(startDate, endDate);
    if (startDate && endDate) {
      let startTimestamp = new Date(startDate).getTime();
      let endTimestamp = new Date(endDate).getTime();
      setIsButtonDisabled(endTimestamp > startTimestamp ? false : true);
      console.log(isButtonDisabled);
    }
  };
  
  const handleSubmit = () =>{
    onDateChange(startDate, endDate);
    setIsButtonDisabled(true);;
  }

  return (
    <div className="selection-container">
      <Select
        sx={{
          width: 150,
          height: 50,
          backgroundColor: "#e1e3e3",
          borderRadius: "8px",
          borderColor: "GrayText",
          fontSize: "18px",
          fontFamily: "Tahoma, Verdana, Segoe, sans-serif",
        }}
        value={selectedValue}
        onChange={handleSelectChange}
        className="selectState"
      >
        <MenuItem value="Real-time">Real-time</MenuItem>
        <MenuItem value="Last-Week">Last Week</MenuItem>
        <MenuItem value="Last-Month">Last Month</MenuItem>
        <MenuItem value="Custom">Custom</MenuItem>
      </Select>
      {!isRealTime && isCustom && (
        <div className="date-picker-container">
          <div className="date-picker-wrapper">
            <label>Start Date:</label>
            <DatePicker
              showTimeSelect
              selected={startDate}
              onChange={(date) => handleDateChange(date, "start")}
              timeFormat="HH:mm"
              timeIntervals={15}
              dateFormat="dd/MM/yyyy, HH:mm"
              minDate={new Date(2023, 4, 15)}
              maxDate={new Date()}
              className="date-picker"
            />
            <label>End Date:</label>
            <DatePicker
              showTimeSelect
              selected={endDate}
              onChange={(date) => handleDateChange(date, "end")}
              timeFormat="HH:mm"
              timeIntervals={15}
              dateFormat="dd/MM/yyyy, HH:mm"
              minDate={new Date(2023, 4, 15)}
              maxDate={new Date()}
              className="date-picker"
            />
          </div>
          <Button
            onClick={handleSubmit}
            disabled={isButtonDisabled}
          >
            Submit
          </Button>
        </div>
      )}
    </div>
  );
};

export default TimeSelection;
