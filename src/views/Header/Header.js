import logo from "./Logo.png";
import { getRouterList } from "../../RouterList";
import { Link } from "react-router-dom";
import Cookies from "js-cookie";
import { useNavigate } from "react-router-dom";
import { Modal } from "bootstrap";
import { useState, useRef } from "react";
import { getData } from "../../Components/Common/FuncAxios.js";
import UserInfor from "../../Components/UserManage/UserInfor.js";

const Header = () => {
  const navigate = useNavigate();
  const [ShowModal, SetShowModal] = useState("");
  const [userInfor, setUserInfor] = useState({});
  const [modal, setModal] = useState(null);
  const parseExceptionModal = useRef();

  const showModalForm = () => {
    const modal = new Modal(parseExceptionModal.current, {
      keyboard: false,
      backdrop: "static",
    });
    setModal(modal);
    modal.show();
  };

  const hideModal = () => {
    modal.hide();
  };

  const handleOnclickSignOut = () => {
    Object.keys(Cookies.get()).forEach(function (cookieName) {
      var neededAttributes = {
        // Here you pass the same attributes that were used when the cookie was created
        // and are required when removing the cookie
      };
      Cookies.remove(cookieName);
    });
    navigate("/login");
  };

  const handleOnInforClick = async () => {
    const getUser = await getData(
      `User/GetUserByName?username=${Cookies.get("username")}`
    );
    setUserInfor(getUser);
  };

  return (
    <>
      <nav className="main-header navbar navbar-expand-md navbar-light navbar-white">
        <div className="container" style={{ maxWidth: "100%" }}>
          <Link to="/" className="navbar-brand">
            <img
              src={logo}
              alt="TBSL Logo"
              className="brand-image"
              style={{ opacity: ".8" }}
            />
          </Link>

          <button
            className="navbar-toggler order-1"
            type="button"
            data-toggle="collapse"
            data-target="#navbarCollapse"
            aria-controls="navbarCollapse"
            aria-expanded="false"
            aria-label="Toggle navigation"
          >
            <span className="navbar-toggler-icon" />
          </button>
          <div className="collapse navbar-collapse order-3" id="navbarCollapse">
            <ul className="navbar-nav">
              <li className="nav-item"></li>

              {Cookies.get("AccType") === "NV" ? (
                <>
                  {getRouterList() &&
                    getRouterList().length > 0 &&
                    getRouterList().map((val, index) => {
                      return (
                        <li key={index} className="nav-item dropdown">
                          <a
                            id="dropdownSubMenu1"
                            href="#"
                            data-toggle="dropdown"
                            aria-haspopup="true"
                            aria-expanded="false"
                            className="nav-link dropdown-toggle"
                          >
                            {val.name}
                          </a>
                          <ul
                            aria-labelledby="dropdownSubMenu1"
                            className="dropdown-menu border-0 shadow"
                          >
                            {val.child &&
                              val.child.length > 0 &&
                              val.child.map((value, num) => {
                                return (
                                  <li key={num}>
                                    <Link
                                      to={value.pathName}
                                      className="dropdown-item"
                                    >
                                      {value.name}
                                    </Link>
                                  </li>
                                );
                              })}
                          </ul>
                        </li>
                      );
                    })}
                </>
              ) : (
                <>
                  <li className="nav-item dropdown">
                    <a
                      id="dropdownSubMenu1"
                      href="#"
                      data-toggle="dropdown"
                      aria-haspopup="true"
                      aria-expanded="false"
                      className="nav-link dropdown-toggle"
                    >
                      Xem Thông Tin
                    </a>
                    <ul
                      aria-labelledby="dropdownSubMenu1"
                      className="dropdown-menu border-0 shadow"
                    >
                      <li>
                        <Link to={"/transport"} className="dropdown-item">
                          Thông Tin Vận Đơn
                        </Link>
                      </li>
                    </ul>
                  </li>
                </>
              )}
            </ul>
          </div>
          {/* Right navbar links */}
          <ul className="order-1 order-md-3 navbar-nav navbar-no-expand ml-auto">
            {/* <li className="nav-item dropdown">
              <a className="nav-link" data-toggle="dropdown" href="#">
                <i className="far fa-bell" />
                <span className="badge badge-warning navbar-badge">15</span>
              </a>
              <div className="dropdown-menu dropdown-menu-lg dropdown-menu-right">
                <span className="dropdown-header">15 Notifications</span>
                <div className="dropdown-divider" />
                <a href="#" className="dropdown-item">
                  <i className="fas fa-envelope mr-2" /> 4 new messages
                  <span className="float-right text-muted text-sm">3 mins</span>
                </a>
                <div className="dropdown-divider" />
                <a href="#" className="dropdown-item">
                  <i className="fas fa-users mr-2" /> 8 friend requests
                  <span className="float-right text-muted text-sm">
                    12 hours
                  </span>
                </a>
                <div className="dropdown-divider" />
                <a href="#" className="dropdown-item">
                  <i className="fas fa-file mr-2" /> 3 new reports
                  <span className="float-right text-muted text-sm">2 days</span>
                </a>
                <div className="dropdown-divider" />
                <a href="#" className="dropdown-item dropdown-footer">
                  See All Notifications
                </a>
              </div>
            </li>
            <li className="nav-item dropdown">
              <a className="nav-link" data-toggle="dropdown" href="#">
                <i className="fas fa-comments" />
                <span className="badge badge-danger navbar-badge">3</span>
              </a>
              <div className="dropdown-menu dropdown-menu-lg dropdown-menu-right">
                <a href="#" className="dropdown-item">
                  <div className="media">
                    <div className="media-body">
                      <h3 className="dropdown-item-title">
                        Brad Diesel
                        <span className="float-right text-sm text-danger">
                          <i className="fas fa-star" />
                        </span>
                      </h3>
                      <p className="text-sm">Call me whenever you can...</p>
                      <p className="text-sm text-muted">
                        <i className="far fa-clock mr-1" /> 4 Hours Ago
                      </p>
                    </div>
                  </div>
                </a>
                <div className="dropdown-divider" />
              </div>
            </li>
            <li className="nav-item dropdown">
              <a className="nav-link" data-toggle="dropdown" href="#">
                <i className="flag-icon flag-icon-vn" />
              </a>
              <div className="dropdown-menu dropdown-menu-right p-0">
                <a href="#" className="dropdown-item active">
                  <i className="flag-icon flag-icon-vn mr-2" /> Tiếng Việt
                </a>
                <a href="#" className="dropdown-item">
                  <i className="flag-icon flag-icon-us mr-2" /> English
                </a>
              </div>
            </li> */}
            <li className="nav-item dropdown user-menu">
              <a
                href="#"
                className="nav-link dropdown-toggle"
                data-toggle="dropdown"
              >
                <i className="fas fa-user"></i>
              </a>
              <ul className="dropdown-menu dropdown-menu-lg dropdown-menu-right">
                {/* User image */}
                <li className="user-body text-center">
                  <p>{Cookies.get("fullname")}</p>
                  <p>{Cookies.get("BoPhan")}</p>
                </li>
                {/* Menu Footer*/}
                <li className="user-footer">
                  <button
                    className="btn btn-default btn-flat"
                    onClick={() =>
                      handleOnInforClick(
                        showModalForm(SetShowModal("UserInformation"))
                      )
                    }
                  >
                    Thông Tin
                  </button>
                  <button
                    onClick={() => handleOnclickSignOut()}
                    className="btn btn-default btn-flat float-right"
                  >
                    Đăng Xuất
                  </button>
                </li>
              </ul>
            </li>
          </ul>
        </div>
      </nav>

      <div
        className="modal fade"
        id="modal-xl"
        data-backdrop="static"
        ref={parseExceptionModal}
        aria-labelledby="parseExceptionModal"
        backdrop="static"
      >
        <div
          className="modal-dialog modal-dialog-scrollable"
          style={{ maxWidth: "80%" }}
        >
          <div className="modal-content">
            <div className="modal-header">
              <button
                type="button"
                className="close"
                data-dismiss="modal"
                onClick={() => hideModal()}
                aria-label="Close"
              >
                <span aria-hidden="true">×</span>
              </button>
            </div>
            <div className="modal-body">
              <>
                {ShowModal === "UserInformation" && (
                  <UserInfor
                    userInformation={userInfor}
                    hideModal={hideModal}
                  />
                )}
              </>
            </div>
          </div>
        </div>
      </div>
    </>
  );
};

export default Header;
