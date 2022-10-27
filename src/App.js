import Header from "../src/views/Header/Header";
import { Routes, Route } from "react-router-dom";
import HomePage from "./Components/Home/HomePage";
import AddressPage from "./Components/AddressManage/AddressPage";
import DriverPage from "./Components/DriverManage/DriverPage";
import CustommerPage from "./Components/CustommerManage/CustommerPage";
import { ToastContainer } from "react-toastify";
import RoadPage from "./Components/RoadManage/RoadPage";
import ContractPage from "./Components/ContractManage/ContractPage";

import "react-toastify/dist/ReactToastify.css";
import "react-datepicker/dist/react-datepicker.css";
import "react-tabs/style/react-tabs.css";
import PriceTablePage from "./Components/PriceListManage/PriceTablePage";
import TransportPage from "./Components/Transport/TransportPage";
import ProductServicePage from "./Components/ProductService/ProductServicePage";
import VehiclePage from "./Components/VehicleManage/VehiclePage";
import RomoocPage from "./Components/RomoocManage/RomoocPage";

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
                  <Route path="/road" element={<RoadPage />} />
                  <Route path="/contract" element={<ContractPage />} />
                  <Route path="/pricetable" element={<PriceTablePage />} />
                  <Route path="/transport" element={<TransportPage />} />
                  <Route path="/vehicle" element={<VehiclePage />} />
                  <Route path="/romooc" element={<RomoocPage />} />
                  <Route
                    path="/productService"
                    element={<ProductServicePage />}
                  />
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
