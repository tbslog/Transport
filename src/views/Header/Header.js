import logo from "./Logo.png";
import { getRouterList } from "../../RouterList";
import { Link } from "react-router-dom";

const Header = () => {
  return (
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
          </ul>
        </div>
        {/* Right navbar links */}
        <ul className="order-1 order-md-3 navbar-nav navbar-no-expand ml-auto">
          {/* Notifications Dropdown Menu */}
          <li className="nav-item dropdown">
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
                <span className="float-right text-muted text-sm">12 hours</span>
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
          {/* Messages Dropdown Menu */}
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
          {/* lang dropdown menu */}
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
          </li>
          {/* user dropdown menu */}
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
                <p>Alexander Pierce</p>
                <p>IT</p>
              </li>
              {/* Menu Footer*/}
              <li className="user-footer">
                <a href="#" className="btn btn-default btn-flat">
                  Profile
                </a>
                <a href="#" className="btn btn-default btn-flat float-right">
                  Sign out
                </a>
              </li>
            </ul>
          </li>
        </ul>
      </div>
    </nav>
  );
};

export default Header;
