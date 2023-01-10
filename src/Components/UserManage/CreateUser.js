import { useState, useEffect, useMemo } from "react";
import { getData, postData, getDataCustom } from "../Common/FuncAxios";
import { useForm, Controller } from "react-hook-form";
import Select from "react-select";

const CreateUser = (props) => {
  const { getListUser } = props;
  const {
    register,
    reset,
    setValue,
    control,
    formState: { errors },
    handleSubmit,
  } = useForm({
    mode: "onChange",
  });

  const Validate = {
    AccountType: {
      required: "Không được để trống",
    },
    MaNhanVien: {
      minLength: {
        value: 5,
        message: "Không được ít hơn 5 ký tự",
      },
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt",
      },
    },
    FullName: {
      required: "Không được để trống",
      maxLength: {
        value: 40,
        message: "Không được vượt quá 40 ký tự",
      },
      minLength: {
        value: 2,
        message: "Không được ít hơn 2 ký tự",
      },
    },
    UserName: {
      required: "Không được để trống",
      maxLength: {
        value: 30,
        message: "Không được vượt quá 30 ký tự",
      },
      minLength: {
        value: 3,
        message: "Không được ít hơn 3 ký tự",
      },
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[a-z0-9.]+(?<![_.])$/,
        message:
          "Vui lòng không ký tự đặc biệt, không viết dấu ,không khoảng trống cho tên đăng nhập",
      },
    },
    Password: {
      maxLength: {
        value: 30,
        message: "Không được vượt quá 30 ký tự",
      },
      minLength: {
        value: 6,
        message: "Không được ít hơn 6 ký tự",
      },
      required: "Không được để trống",
    },
    Role: {
      required: "Không được để trống",
    },
    TrangThai: {
      required: "Không được để trống",
    },
  };

  const [IsLoading, SetIsLoading] = useState(true);
  const [listRole, setListRole] = useState([]);
  const [listBP, setListBP] = useState([]);
  const [listStatus, setListStatus] = useState([]);

  useEffect(() => {
    (async () => {
      SetIsLoading(true);
      let getStatusList = await getDataCustom(`Common/GetListStatus`, [
        "common",
      ]);
      setListStatus(getStatusList);

      let getListRole = await getData("User/GetListRoleSelect");
      if (getListRole && getListRole.length > 0) {
        let obj = [];

        getListRole.map((val) => {
          obj.push({
            label: val.roleName,
            value: val.id,
          });
        });
        setListRole(obj);
      }

      let getListDeparment = await getData("User/GetListDepartmentSelect");
      if (getListDeparment && getListDeparment.length > 0) {
        let obj = [];

        getListDeparment.map((val) => {
          obj.push({
            label: val.tenBoPhan,
            value: val.maBoPhan,
          });
        });
        setListBP(obj);
      }
      SetIsLoading(false);
    })();
  }, []);

  const handleResetClick = () => {
    reset();
  };

  const onSubmit = async (data) => {
    SetIsLoading(true);

    const creaetUser = await postData("User/CreateUser", {
      UserName: data.UserName,
      PassWord: data.Password,
      AccountType: data.AccountType,
      HoVaTen: data.FullName,
      MaNhanVien: data.MaNhanVien,
      MaBoPhan: !data.BoPhan ? null : data.BoPhan.value,
      RoleId: data.Role.value,
      TrangThai: data.TrangThai,
    });

    if (creaetUser === 1) {
      getListUser(1);
      reset();
    }
    SetIsLoading(false);
  };

  return (
    <div className="card card-primary">
      <div className="card-header">
        <h3 className="card-title">Form Thêm Người Dùng</h3>
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
                    {...register(`MaNhanVien`, Validate.MaNhanVien)}
                  />
                  {errors.MaNhanVien && (
                    <span className="text-danger">
                      {errors.MaNhanVien.message}
                    </span>
                  )}
                </div>
              </div>
              <div className="col col-sm">
                <div className="form-group">
                  <label htmlFor="FullName">Họ Và Tên(*)</label>
                  <input
                    type="text"
                    className="form-control"
                    id="FullName"
                    {...register(`FullName`, Validate.FullName)}
                  />
                  {errors.FullName && (
                    <span className="text-danger">
                      {errors.FullName.message}
                    </span>
                  )}
                </div>
              </div>
              <div className="col col-sm">
                <div className="form-group">
                  <label htmlFor="BoPhan">Bộ Phận</label>
                  <Controller
                    name="BoPhan"
                    rules={Validate.BoPhan}
                    control={control}
                    render={({ field }) => (
                      <Select
                        {...field}
                        classNamePrefix={"form-control"}
                        value={field.value}
                        options={listBP}
                      />
                    )}
                  />
                  {errors.BoPhan && (
                    <span className="text-danger">{errors.BoPhan.message}</span>
                  )}
                </div>
              </div>
            </div>

            <div className="row">
              <div className="col col-sm">
                <div className="form-group">
                  <label htmlFor="Username">Tài Khoản(*)</label>
                  <input
                    type="text"
                    className="form-control"
                    id="UserName"
                    {...register(`UserName`, Validate.UserName)}
                  />
                  {errors.UserName && (
                    <span className="text-danger">
                      {errors.UserName.message}
                    </span>
                  )}
                </div>
              </div>
              <div className="col col-sm">
                <div className="form-group">
                  <label htmlFor="Password">Mật Khẩu(*)</label>
                  <input
                    type="password"
                    className="form-control"
                    id="Password"
                    {...register(`Password`, Validate.Password)}
                  />
                  {errors.Password && (
                    <span className="text-danger">
                      {errors.Password.message}
                    </span>
                  )}
                </div>
              </div>
            </div>
            <div className="row">
              <div className="col col-sm">
                <div className="form-group">
                  <label htmlFor="AccountType">Loại Account(*)</label>
                  <select
                    className="form-control"
                    {...register(`AccountType`, Validate.AccountType)}
                  >
                    <option value={"NV"}>Nhân Viên</option>
                    <option value={"KH"}>Khách Hàng</option>
                  </select>
                  {errors.TrangThai && (
                    <span className="text-danger">
                      {errors.TrangThai.message}
                    </span>
                  )}
                </div>
              </div>
              <div className="col col-sm">
                <div className="form-group">
                  <label htmlFor="Role">Phân Quyền(*)</label>
                  <Controller
                    name="Role"
                    control={control}
                    render={({ field }) => (
                      <Select
                        {...field}
                        classNamePrefix={"form-control"}
                        value={field.value}
                        options={listRole}
                      />
                    )}
                    rules={Validate.Role}
                  />
                  {errors.Role && (
                    <span className="text-danger">{errors.Role.message}</span>
                  )}
                </div>
              </div>
              <div className="col col-sm">
                <div className="form-group">
                  <label htmlFor="TrangThai">Trạng Thái(*)</label>
                  <select
                    className="form-control"
                    {...register(`TrangThai`, Validate.TrangThai)}
                  >
                    {listStatus &&
                      listStatus.map((val) => {
                        return (
                          <option value={val.statusId} key={val.statusId}>
                            {val.statusContent}
                          </option>
                        );
                      })}
                  </select>
                  {errors.TrangThai && (
                    <span className="text-danger">
                      {errors.TrangThai.message}
                    </span>
                  )}
                </div>
              </div>
            </div>
          </div>
          <div className="card-footer">
            <div>
              <button
                type="button"
                onClick={() => handleResetClick()}
                className="btn btn-warning"
              >
                Làm mới
              </button>
              <button
                type="submit"
                className="btn btn-primary"
                style={{ float: "right" }}
              >
                Thêm mới
              </button>
            </div>
          </div>
        </form>
      )}
    </div>
  );
};

export default CreateUser;
