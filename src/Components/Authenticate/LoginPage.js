import { useState } from "react";
import Logo from "../../Image/Logo/logo2x.png";
import { getData, postLogin } from "../Common/FuncAxios";
import { useForm } from "react-hook-form";
import Cookies from "js-cookie";
import { useNavigate } from "react-router-dom";
import md5 from "md5";

const LoginPage = () => {
  const { register, handleSubmit } = useForm({
    mode: "onChange",
  });
  const navigate = useNavigate();
  const Validate = {};

  const onSubmit = async (data) => {
    var login = await postLogin("User/Login", {
      UserName: data.UserName,
      Password: md5(data.Password),
    });

    if (login.isSuccess === 1) {
      const getUser = await getData(
        `User/GetUserByName?username=${data.UserName}`
      );

      Cookies.set("username", data.UserName);
      Cookies.set("fullname", getUser.hoVaTen);
      Cookies.set("BoPhan", getUser.tenBoPhan);
      Cookies.set("AccType", getUser.accountType);

      navigate("/");
      window.location.reload();
    } else {
      setErrMsg(login.Message);
    }
  };

  const [errMsg, setErrMsg] = useState("");

  return (
    <div className="hold-transition login-page">
      <div className="login-box">
        {/* /.login-logo */}
        <div className="card card-outline card-primary">
          <div className="card-header text-center">
            <img src={Logo}></img>
          </div>
          <div className="card-body">
            <p
              className="login-box-msg"
              style={{ color: "red", fontWeight: "bold" }}
            >
              {errMsg}
            </p>
            <form onSubmit={handleSubmit(onSubmit)}>
              <div className="input-group mb-3">
                <input
                  type="text"
                  className="form-control"
                  placeholder="Tên Đăng Nhập"
                  {...register("UserName", Validate.UserName)}
                />
                <div className="input-group-append">
                  <div className="input-group-text">
                    <span className="fas fa-user" />
                  </div>
                </div>
              </div>
              <div className="input-group mb-3">
                <input
                  type="password"
                  className="form-control"
                  placeholder="Mật Khẩu"
                  {...register("Password", Validate.Password)}
                />
                <div className="input-group-append">
                  <div className="input-group-text">
                    <span className="fas fa-lock" />
                  </div>
                </div>
              </div>
              <div className="row">
                {/* /.col */}
                <div className="col-12">
                  <button type="submit" className="btn btn-primary btn-block">
                    Đăng Nhập
                  </button>
                </div>
                {/* /.col */}
              </div>
            </form>
          </div>
          {/* /.card-body */}
        </div>
        {/* /.card */}
      </div>
    </div>
  );
};

export default LoginPage;
