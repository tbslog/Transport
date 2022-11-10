import { Routes, Route } from "react-router-dom";
import LoginPage from "./Components/Authenticate/LoginPage";

import "react-toastify/dist/ReactToastify.css";
import "react-datepicker/dist/react-datepicker.css";
import "react-tabs/style/react-tabs.css";
import PrivateRoutes from "./Components/utils/PrivateRoutes";
import PriceTablePage from "./Components/PriceListManage/PriceTablePage";
import TransportPage from "./Components/Transport/TransportPage";
import ProductServicePage from "./Components/ProductService/ProductServicePage";
import VehiclePage from "./Components/VehicleManage/VehiclePage";
import RomoocPage from "./Components/RomoocManage/RomoocPage";
import SubFeePage from "./Components/SubFee/SubFeePage";
import UserPage from "./Components/UserManage/UserPage";
import RolePage from "./Components/RoleManage/RolePage";
import RoadPage from "./Components/RoadManage/RoadPage";
import ContractPage from "./Components/ContractManage/ContractPage";
import HomePage from "./Components/Home/HomePage";
import AddressPage from "./Components/AddressManage/AddressPage";
import DriverPage from "./Components/DriverManage/DriverPage";
import CustommerPage from "./Components/CustommerManage/CustommerPage";
import HandlingPage from "./Components/Transport/HandlingPage";

function App() {
  return (
    <div className="App">
      <header className="App-header">
        <>
          <Routes>
            <Route element={<PrivateRoutes />}>
              <Route path="/" element={<HomePage />} exact />
              <Route path="/custommer" element={<CustommerPage />} />
              <Route path="/address" element={<AddressPage />} />
              <Route path="/driver" element={<DriverPage />} />
              <Route path="/road" element={<RoadPage />} />
              <Route path="/contract" element={<ContractPage />} />
              <Route path="/pricetable" element={<PriceTablePage />} />
              <Route path="/transport" element={<TransportPage />} />
              <Route path="/vehicle" element={<VehiclePage />} />
              <Route path="/romooc" element={<RomoocPage />} />
              <Route path="/subfee" element={<SubFeePage />} />
              <Route path="/user" element={<UserPage />} />
              <Route path="/role" element={<RolePage />} />
              <Route path="/handling" element={<HandlingPage />} />
              <Route path="/productService" element={<ProductServicePage />} />
            </Route>
            <Route element={<LoginPage />} path="/login"></Route>
          </Routes>
        </>
      </header>
    </div>
  );
}

export default App;
