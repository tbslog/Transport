import { useState, useEffect } from "react";
import { useForm, Controller } from "react-hook-form";
import { postData } from "../Common/FuncAxios";
import Cookies from "js-cookie";
import md5 from "md5";

const UserInfor = (props) => {
  const { userInformation, hideModal } = props;
  const {
    register,
    setValue,
    reset,
    control,
    formState: { errors },
    handleSubmit,
  } = useForm({
    mode: "onChange",
  });

  const Validate = {
    Password: {
      maxLength: {
        value: 30,
        message: "Không được vượt quá 30 ký tự",
      },
      minLength: {
        value: 6,
        message: "Không được ít hơn 6 ký tự",
      },
    },
  };

  const [IsLoading, SetIsLoading] = useState(false);

  useEffect(() => {
    if (props && userInformation && Object.keys(userInformation).length > 0) {
      setValue("MaNhanVien", userInformation.maNhanVien);
      setValue("FullName", userInformation.hoVaTen);
      setValue("BoPhan", userInformation.tenBoPhan);
      setValue("UserName", Cookies.get("username"));
      setValue("RoleName", userInformation.roleName);
    }
  }, [props, userInformation]);

  const onSubmit = async (data) => {
    SetIsLoading(true);

    const updateUser = await postData(
      `User/ChangePassword?username=${Cookies.get("username")}`,
      {
        OldPassword: md5(data.OldPassword),
        NewPassword: md5(data.NewPassword),
        ReNewPassword: md5(data.reNewPassword),
      }
    );

    if (updateUser === 1) {
      reset();
      hideModal();
    }

    SetIsLoading(false);
  };

  return (
    <div className="card card-primary">
      <div className="card-header">
        <h3 className="card-title">Thông Tin Người Dùng</h3>
      </div>
      <div>{IsLoading === true && <div>Loading...</div>}</div>

      {IsLoading === false && (
        <form onSubmit={handleSubmit(onSubmit)}>
          <div className="card-body">
            <div className="row">
              <div className="col col-sm">
                <div className="form-group">
                  <label htmlFor="MaNhanVien">Mã Nhân Viên</label>
                  <input
                    type="text"
                    className="form-control"
                    id="MaNhanVien"
                    readOnly={true}
                    {...register(`MaNhanVien`)}
                  />
                </div>
              </div>
              <div className="col col-sm">
                <div className="form-group">
                  <label htmlFor="FullName">Họ Và Tên</label>
                  <input
                    type="text"
                    className="form-control"
                    id="FullName"
                    readOnly={true}
                    {...register(`FullName`)}
                  />
                </div>
              </div>
              <div className="col col-sm">
                <div className="form-group">
                  <label htmlFor="BoPhan">Bộ Phận</label>
                  <input
                    type="text"
                    className="form-control"
                    id="BoPhan"
                    readOnly={true}
                    {...register(`BoPhan`)}
                  />
                </div>
              </div>
            </div>
            <div className="row">
              <div className="col col-sm">
                <div className="form-group">
                  <label htmlFor="Username">Tài Khoản</label>
                  <input
                    type="text"
                    className="form-control"
                    id="UserName"
                    readOnly={true}
                    {...register(`UserName`)}
                  />
                </div>
              </div>
              <div className="col col-sm">
                <div className="form-group">
                  <label htmlFor="RoleName">Phân Quyền</label>
                  <input
                    type="text"
                    className="form-control"
                    id="RoleName"
                    readOnly={true}
                    {...register(`RoleName`)}
                  />
                </div>
              </div>
            </div>
            <div className="row">
              <div className="col col-sm">
                <div className="form-group">
                  <label htmlFor="OldPassword">Mật Khẩu Cũ</label>
                  <input
                    type="password"
                    className="form-control"
                    id="OldPassword"
                    {...register(`OldPassword`, Validate.OldPassword)}
                  />
                  {errors.OldPassword && (
                    <span className="text-danger">
                      {errors.OldPassword.message}
                    </span>
                  )}
                </div>
              </div>
              <div className="col col-sm">
                <div className="form-group">
                  <label htmlFor="NewPassword">Mật Khẩu Mới</label>
                  <input
                    type="password"
                    className="form-control"
                    id="NewPassword"
                    {...register(`NewPassword`, Validate.NewPassword)}
                  />
                  {errors.NewPassword && (
                    <span className="text-danger">
                      {errors.NewPassword.message}
                    </span>
                  )}
                </div>
              </div>
              <div className="col col-sm">
                <div className="form-group">
                  <label htmlFor="reNewPassword">Nhập Lại Mật Khẩu Mới</label>
                  <input
                    type="password"
                    className="form-control"
                    id="reNewPassword"
                    {...register(`reNewPassword`, Validate.reNewPassword)}
                  />
                  {errors.reNewPassword && (
                    <span className="text-danger">
                      {errors.reNewPassword.message}
                    </span>
                  )}
                </div>
              </div>
            </div>
          </div>
          <div className="card-footer">
            <div>
              <button
                type="submit"
                className="btn btn-primary"
                style={{ float: "right" }}
              >
                Cập Nhật
              </button>
            </div>
          </div>
        </form>
      )}
    </div>
  );
};

export default UserInfor;
