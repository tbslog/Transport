import { useState, useEffect } from "react";
import Header from "../src/views/Header/Header";
import { Routes, Route } from "react-router-dom";
import { getRouterList } from "./RouterList";
import HomePage from "./Components/Home/HomePage";
import AddressPage from "./Components/AddressManage/AddressPage";
import DriverPage from "./Components/DriverManage/DriverPage";
import CustommerPage from "./Components/CustommerManage/CustommerPage";
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";

function App() {
  return (
    <div className="App">
      <header className="App-header">
        <>
          <Header />
          <ToastContainer style={{ width: "auto", height: "auto" }} />
          <div className="content-wrapper">
            <div className="content">
              <div className="container" style={{ maxWidth: "100%" }}>
                <Routes>
                  <Route path="/" element={<HomePage />} />
                  <Route path="/custommer" element={<CustommerPage />} />
                  <Route path="/address" element={<AddressPage />} />
                  <Route path="/driver" element={<DriverPage />} />
                </Routes>
              </div>
            </div>
          </div>
        </>
      </header>
    </div>
  );
}

export default App;
