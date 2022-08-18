import { useState, useEffect } from "react";
import Header from "../src/views/Header/Header";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import { getRouterList } from "./RouterList";
import HomePage from "./Components/HomePage";
import AddressPage from "./Components/AddressManage/AddressPage";
import DriverPage from "./Components/DriverManage/DriverPage";
import CustommerPage from "./Components/CustommerManage/CustommerPage";

function App() {
  return (
    <div className="App">
      <header className="App-header">
        <BrowserRouter>
          <Header />
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
        </BrowserRouter>
      </header>
    </div>
  );
}

export default App;

<div className="content-wrapper">
  <div className="content">
    <div className="container"></div>
  </div>
</div>;
