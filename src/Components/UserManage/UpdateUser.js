import { useState, useEffect, useMemo } from "react";
import { getData, postData, getDataCustom } from "../Common/FuncAxios";
import { useForm, Controller } from "react-hook-form";
import Select from "react-select";
import LoadingPage from "../Common/Loading/LoadingPage";

const UpdateUser = (props) => {
  const { getListUser, selectIdClick, hideModal } = props;
  const {
    register,
    setValue,
    control,
    formState: { errors },
    handleSubmit,
  } = useForm({
    mode: "onChange",
  });

  const Validate = {
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
    },
    Role: {
      required: "Không được để trống",
    },
    TrangThai: {
      required: "Không được để trống",
    },
  };

  const [IsLoading, SetIsLoading] = useState(false);
  const [listRole, setListRole] = useState([]);
  const [listBP, setListBP] = useState([]);
  const [listStatus, setListStatus] = useState([]);

  useEffect(() => {
    (async () => {
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
    })();
  }, []);

  useEffect(() => {
    if (
      selectIdClick &&
      Object.keys(selectIdClick).length > 0 &&
      listRole &&
      listBP &&
      listStatus &&
      listRole.length > 0 &&
      listBP.length > 0 &&
      listStatus.length > 0
    ) {
      setValue("UserName", selectIdClick.userName);
      setValue("FullName", selectIdClick.hoVaTen);
      setValue("MaNhanVien", selectIdClick.maNhanVien);
      setValue("TrangThai", selectIdClick.trangThai);
      setValue("AccountType", selectIdClick.accountType);

      setValue(
        "BoPhan",
        { ...listBP.filter((x) => x.value === selectIdClick.maBoPhan) }[0]
      );

      setValue(
        "SetRole",
        {
          ...listRole.filter((x) => x.value === parseInt(selectIdClick.roleId)),
        }[0]
      );
    }
  }, [selectIdClick, listRole, listBP, listStatus]);

  const onSubmit = async (data) => {
    SetIsLoading(true);

    const updateUser = await postData(
      `User/UpdateUser?id=${selectIdClick.id}`,
      {
        Password: data.Password,
        HoVaTen: data.FullName,
        MaNhanVien: data.MaNhanVien,
        MaBoPhan: !data.BoPhan ? null : data.BoPhan.value,
        RoleId: data.SetRole.value,
        TrangThai: data.TrangThai,
        AccountType: data.AccountType,
      }
    );

    if (updateUser === 1) {
      getListUser(1);
      hideModal();
    }

    SetIsLoading(false);
  };

  return (
    <div className="card card-primary">
      <div>
        {IsLoading === true && (
          <div>
            <LoadingPage></LoadingPage>
          </div>
        )}
      </div>

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
                    disabled={true}
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
                  <label htmlFor="Password">Mật Khẩu</label>
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
                    <option value={"NCC"}>Nhà Cung Cấp</option>
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
                  <label htmlFor="SetRole">Phân Quyền(*)</label>
                  <Controller
                    name="SetRole"
                    control={control}
                    render={({ field }) => (
                      <Select
                        {...field}
                        classNamePrefix={"form-control"}
                        value={field.value}
                        options={listRole}
                      />
                    )}
                    rules={Validate.SetRole}
                  />
                  {errors.SetRole && (
                    <span className="text-danger">
                      {errors.SetRole.message}
                    </span>
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

export default UpdateUser;
