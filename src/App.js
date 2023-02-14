import { Routes, Route } from "react-router-dom";
import LoginPage from "./Components/Authenticate/LoginPage";
import Cookies from "js-cookie";

import "react-toastify/dist/ReactToastify.css";
import "react-datepicker/dist/react-datepicker.css";
import "react-tabs/style/react-tabs.css";
import "./Css/ButtonTitle.scss";
import "./Css/Datatable.scss";

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
import BillPage from "./Components/Bill/BillPage";
import ReportPage from "./Components/Report/ReportPage";
import HandlingPageNew from "./Components/Transport/HandlingPageNew";

function App() {
  const accountType = Cookies.get("AccType");

  return (
    <div className="App">
      <header className="App-header">
        <Routes>
          <Route element={<PrivateRoutes />}>
            {accountType && accountType === "NV" && (
              <>
                <Route path="/custommer" element={<CustommerPage />} />
                <Route path="/address" element={<AddressPage />} />
                <Route path="/driver" element={<DriverPage />} />
                <Route path="/road" element={<RoadPage />} />
                <Route path="/contract" element={<ContractPage />} />
                <Route path="/pricetable" element={<PriceTablePage />} />
                <Route path="/vehicle" element={<VehiclePage />} />
                <Route path="/romooc" element={<RomoocPage />} />
                <Route path="/subfee" element={<SubFeePage />} />
                <Route path="/user" element={<UserPage />} />
                <Route path="/role" element={<RolePage />} />
                <Route path="/handlingfull" element={<HandlingPage />} />
                <Route
                  path="/productService"
                  element={<ProductServicePage />}
                />
                <Route path="/bill" element={<BillPage />} />
                <Route path="/report" element={<ReportPage />} />
                <Route path="/handlingless" element={<HandlingPageNew />} />
              </>
            )}
            <Route path="/transport" element={<TransportPage />} />
            <Route path="/" element={<HomePage />} exact />
          </Route>
          <Route element={<LoginPage />} path="/login"></Route>
        </Routes>
      </header>
    </div>
  );
}

export default App;
