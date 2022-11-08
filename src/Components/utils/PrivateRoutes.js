import { Outlet, Navigate } from "react-router-dom";
import Header from "../../views/Header/Header";
import { ToastContainer } from "react-toastify";
import Cookies from "js-cookie";

const PrivateRoutes = () => {
  let auth = { token: Cookies.get("token") };
  return auth.token ? (
    <>
      <Header />
      <ToastContainer style={{ width: "auto", height: "auto" }} />
      <div className="content-wrapper">
        <div className="content">
          <div className="container" style={{ maxWidth: "100%" }}>
            <Outlet />
          </div>
        </div>
      </div>
    </>
  ) : (
    <Navigate to="/login" />
  );
};

export default PrivateRoutes;
