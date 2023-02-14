import { useState, useEffect } from "react";
import { postData } from "../Common/FuncAxios";
import { useForm } from "react-hook-form";
import LoadingPage from "../Common/Loading/LoadingPage";

const EditCustommer = (props) => {
  const [IsLoading, SetIsLoading] = useState(true);
  const {
    register,
    watch,
    setValue,
    formState: { errors },
    handleSubmit,
  } = useForm({
    mode: "onChange",
  });

  const Validate = {
    MaKH: {
      required: "Không được để trống",
      maxLength: {
        value: 8,
        message: "Phải là 8 ký tự",
      },
      minLength: {
        value: 8,
        message: "Phải là 8 ký tự",
      },
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt",
      },
    },
    Chuoi: {
      required: "Không được để trống",
      maxLength: {
        value: 50,
        message: "Không được vượt quá 50 ký tự",
      },
      minLength: {
        value: 1,
        message: "Không được ít hơn 1 ký tự",
      },
    },
    TenKH: {
      required: "Không được để trống",
      maxLength: {
        value: 50,
        message: "Không được vượt quá 50 ký tự",
      },
      minLength: {
        value: 1,
        message: "Không được ít hơn 1 ký tự",
      },
      pattern: {
        value:
          /^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9 aAàÀảẢãÃáÁạẠăĂằẰẳẲẵẴắẮặẶâÂầẦẩẨẫẪấẤậẬbBcCdDđĐeEèÈẻẺẽẼéÉẹẸêÊềỀểỂễỄếẾệỆ fFgGhHiIìÌỉỈĩĨíÍịỊjJkKlLmMnNoOòÒỏỎõÕóÓọỌôÔồỒổỔỗỖốỐộỘơƠờỜởỞỡỠớỚợỢpPqQrRsStTu UùÙủỦũŨúÚụỤưƯừỪửỬữỮứỨựỰvVwWxXyYỳỲỷỶỹỸýÝỵỴzZ]+(?<![_.])$/,
        message: "Tên khách hàng không được chứa ký tự đặc biệt",
      },
    },
    Email: {
      maxLength: {
        value: 100,
        message: "Không được vượt quá 100 ký tự",
      },
      minLength: {
        value: 3,
        message: "Không được ít hơn 3 ký tự",
      },
      pattern: {
        value: /^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$/,
        message: "Không phải Email",
      },
    },
    MST: {
      maxLength: {
        value: 50,
        message: "Không được vượt quá 50 ký tự",
      },
      minLength: {
        value: 1,
        message: "Không được ít hơn 1 ký tự",
      },
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[0-9]+(?<![_.])$/,
        message: "Mã số thuế chỉ được chứa ký tự là số",
      },
    },
    SDT: {
      maxLength: {
        value: 50,
        message: "Không được vượt quá 20 ký tự",
      },
      minLength: {
        value: 10,
        message: "Không được ít hơn 10 ký tự",
      },
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[0-9]+(?<![_.])$/,
        message: "Số điện thoại chỉ được chứa ký tự là số",
      },
    },
  };

  const [listStatus, setListStatus] = useState([]);
  const [listCustomerGroup, setListCustomerGroup] = useState([]);
  const [listCustomerType, setListCustomerType] = useState([]);
  const [listChuoi, setListChuoi] = useState([]);

  const onSubmit = async (data) => {
    SetIsLoading(true);

    const put = await postData(`Customer/UpdateCustomer?Id=${data.MaKH}`, {
      maKh: data.MaKH,
      tenKh: data.TenKH,
      maSoThue: data.MST,
      sdt: data.SDT,
      email: data.Email,
      trangThai: data.TrangThai,
      nhomKh: data.NhomKH,
      loaiKh: data.LoaiKH,
      chuoi: data.Chuoi,
    });

    if (put === 1) {
      props.getListUser();
      props.hideModal();
    }

    SetIsLoading(false);
  };

  useEffect(() => {
    SetIsLoading(true);

    setListCustomerGroup(props.listCusGroup);
    setListCustomerType(props.listCusType);
    setListStatus(props.listStatus);
    setListChuoi(props.listChuoi);

    SetIsLoading(false);
  }, []);

  useEffect(() => {
    if (
      props.selectIdClick &&
      listCustomerGroup &&
      listCustomerType &&
      Object.keys(props.selectIdClick).length > 0
    ) {
      SetIsLoading(true);
      setValue("MaKH", props.selectIdClick.maKh);
      setValue("MST", props.selectIdClick.maSoThue);
      setValue("SDT", props.selectIdClick.sdt);
      setValue("TenKH", props.selectIdClick.tenKh);
      setValue("Email", props.selectIdClick.email);
      setValue("LoaiKH", props.selectIdClick.loaiKH);
      setValue("NhomKH", props.selectIdClick.nhomKH);
      setValue("TrangThai", props.selectIdClick.trangThai);
      setValue("Chuoi", props.selectIdClick.chuoi);

      SetIsLoading(false);
    }
  }, [props.selectIdClick, listCustomerGroup, listCustomerType]);

  return (
    <>
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
                <div className="col-sm">
                  <div className="form-group">
                    <label htmlFor="LoaiKH">Phân Loại Đối Tác(*)</label>
                    <select
                      disabled={true}
                      className="form-control"
                      {...register("LoaiKH", {
                        required: "Không được để trống",
                      })}
                    >
                      <option value="">Chọn Phân Loại Đối Tác</option>
                      {listCustomerType &&
                        listCustomerType.map((val) => {
                          return (
                            <option value={val.maLoaiKh} key={val.maLoaiKh}>
                              {val.tenLoaiKh}
                            </option>
                          );
                        })}
                    </select>
                    {errors.LoaiKH && (
                      <span className="text-danger">
                        {errors.LoaiKH.message}
                      </span>
                    )}
                  </div>
                </div>

                {watch("LoaiKH") && watch("LoaiKH") === "KH" && (
                  <>
                    <div className="col-sm">
                      <div className="form-group">
                        <label htmlFor="Chuoi">Chuỗi(*)</label>
                        <select
                          className="form-control"
                          {...register("Chuoi", {
                            required: "Không được để trống",
                          })}
                        >
                          <option value="">Chọn Chuỗi</option>
                          {listChuoi &&
                            listChuoi.map((val) => {
                              return (
                                <option value={val.maChuoi} key={val.maChuoi}>
                                  {val.tenChuoi}
                                </option>
                              );
                            })}
                        </select>
                        {errors.Chuoi && (
                          <span className="text-danger">
                            {errors.Chuoi.message}
                          </span>
                        )}
                      </div>
                    </div>
                  </>
                )}
                <div className="col-sm">
                  <div className="form-group">
                    <label htmlFor="NhomKH">Nhóm khách hàng(*)</label>
                    <select
                      className="form-control"
                      {...register("NhomKH", {
                        required: "Không được để trống",
                      })}
                    >
                      <option value="">Chọn Nhóm khách hàng</option>
                      {listCustomerGroup &&
                        listCustomerGroup.map((val) => {
                          return (
                            <option value={val.maNhomKh} key={val.maNhomKh}>
                              {val.tenNhomKh}
                            </option>
                          );
                        })}
                    </select>
                    {errors.NhomKH && (
                      <span className="text-danger">
                        {errors.NhomKH.message}
                      </span>
                    )}
                  </div>
                </div>
              </div>
              <div className="row">
                <div className="col-sm">
                  <div className="form-group">
                    <label htmlFor="MaKH">Mã Đối Tác(*)</label>
                    <input
                      readOnly={true}
                      type="text"
                      className="form-control"
                      id="MaKH"
                      placeholder="Nhập tên đối tác"
                      {...register("MaKH", Validate.MaKH)}
                    />
                    {errors.MaKH && (
                      <span className="text-danger">{errors.MaKH.message}</span>
                    )}
                  </div>
                </div>
                <div className="col-sm">
                  <div className="form-group">
                    <label htmlFor="TenKH">Tên đối tác(*)</label>
                    <input
                      type="text"
                      className="form-control"
                      id="TenKH"
                      placeholder="Nhập tên đối tác"
                      {...register("TenKH", Validate.TenKH)}
                    />
                    {errors.TenKH && (
                      <span className="text-danger">
                        {errors.TenKH.message}
                      </span>
                    )}
                  </div>
                </div>
              </div>
              <div className="row">
                <div className="col-sm">
                  <div className="form-group">
                    <label htmlFor="Email">Địa chỉ Email(*)</label>
                    <input
                      type="text "
                      className="form-control"
                      id="Email"
                      placeholder="Nhập địa chỉ Email"
                      {...register("Email", Validate.Email)}
                    />
                    {errors.Email && (
                      <span className="text-danger">
                        {errors.Email.message}
                      </span>
                    )}
                  </div>
                </div>
                <div className="col-sm">
                  <div className="form-group">
                    <label htmlFor="MST">Mã số thuế(*)</label>
                    <input
                      type="text "
                      className="form-control"
                      id="MST"
                      placeholder="Nhập mã số thuế"
                      {...register("MST", Validate.MST)}
                    />
                    {errors.MST && (
                      <span className="text-danger">{errors.MST.message}</span>
                    )}
                  </div>
                </div>
                <div className="col-sm">
                  <div className="form-group">
                    <label htmlFor="SDT">Số điện thoại(*)</label>
                    <input
                      type="text"
                      className="form-control"
                      id="SDT"
                      placeholder="Nhập số điện thoại"
                      {...register("SDT", Validate.SDT)}
                    />
                    {errors.SDT && (
                      <span className="text-danger">{errors.SDT.message}</span>
                    )}
                  </div>
                </div>
              </div>

              <div className="row">
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="TrangThai">Trạng thái(*)</label>
                    <select
                      className="form-control"
                      {...register("TrangThai", {
                        required: "Không được để trống",
                      })}
                    >
                      <option value="">Chọn trạng thái</option>
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
    </>
  );
};

export default EditCustommer;
