import { useState, useEffect } from "react";
import { getData, postData } from "../Common/FuncAxios";
import { useForm, Controller } from "react-hook-form";
import Select from "react-select";
import DatePicker from "react-datepicker";

const AddPriceTable = (props) => {
  const { getListPriceTable } = props;
  const [IsLoading, SetIsLoading] = useState(true);
  const {
    register,
    reset,
    setValue,
<<<<<<< HEAD
=======
    getValues,
>>>>>>> 103083c6cb18e0f69c01e255502e2c9c68ee3a6f
    control,
    formState: { errors },
    handleSubmit,
  } = useForm({
    mode: "onChange",
  });

  const Validate = {
    MaBangGia: {
      required: "Không được để trống",
      maxLength: {
        value: 10,
        message: "Không được vượt quá 10 ký tự",
      },
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt",
      },
    },
    MaHopDong: {
      required: "Không được để trống",
      maxLength: {
        value: 10,
        message: "Không được vượt quá 10 ký tự",
      },
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt",
      },
    },
    MaKh: {
      required: "Không được để trống",
      maxLength: {
        value: 8,
        message: "Không được vượt quá 8 ký tự",
      },
      minLength: {
        value: 8,
        message: "Không được ít hơn 8 ký tự",
      },
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt",
      },
    },
    MaCungDuong: {
      required: "Không được để trống",
      maxLength: {
        value: 10,
        message: "Không được vượt quá 10 ký tự",
      },
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt",
      },
    },
    NgayApDung: {
      required: "Không được để trống",
      maxLength: {
        value: 10,
        message: "Không được vượt quá 10 ký tự",
      },
      minLength: {
        value: 10,
        message: "Không được ít hơn 10 ký tự",
      },
      pattern: {
        value:
          /^(?:(?:31(\/|-|\.)(?:0?[13578]|1[02]))\1|(?:(?:29|30)(\/|-|\.)(?:0?[13-9]|1[0-2])\2))(?:(?:1[6-9]|[2-9]\d)?\d{2})$|^(?:29(\/|-|\.)0?2\3(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00))))$|^(?:0?[1-9]|1\d|2[0-8])(\/|-|\.)(?:(?:0?[1-9])|(?:1[0-2]))\4(?:(?:1[6-9]|[2-9]\d)?\d{2})$/,
        message: "Không phải định dạng ngày",
      },
    },
    GiaVND: {
      pattern: {
<<<<<<< HEAD
        value: /^[0-9]*$/,
=======
        value: /^[1-9.]*$/,
>>>>>>> 103083c6cb18e0f69c01e255502e2c9c68ee3a6f
        message: "Chỉ được nhập ký tự là số",
      },
      required: "Không được để trống",
    },
    GiaUSD: {
      pattern: {
<<<<<<< HEAD
        value: /^[0-9]*$/,
=======
        value: /^[1-9.]*$/,
>>>>>>> 103083c6cb18e0f69c01e255502e2c9c68ee3a6f
        message: "Chỉ được nhập ký tự là số",
      },
      required: "Không được để trống",
    },
    SoLuong: {
      required: "Không được để trống",
      pattern: {
<<<<<<< HEAD
        value: /^[1-9][0-9]*$/,
=======
        value: /^[1-9]*$/,
>>>>>>> 103083c6cb18e0f69c01e255502e2c9c68ee3a6f
        message: "Chỉ được nhập ký tự là số",
      },
    },
    MaLoaiPhuongTien: {
      required: "Không được để trống",
    },
    MaLoaiHangHoa: {
      required: "Không được để trống",
    },
    MaDVT: {
      required: "Không được để trống",
    },
    MaPTVC: {
      required: "Không được để trống",
    },
    TrangThai: {
      required: "Không được để trống",
    },
  };

  const [listRoad, setListRoad] = useState([]);
  const [listCustomer, setListCustomer] = useState([]);
  const [listVehicleType, setListVehicleType] = useState([]);
  const [listGoodsType, setListGoodsType] = useState([]);
  const [listDVT, setListDVT] = useState([]);
  const [listTransportType, setListTransportType] = useState([]);
  const [listStatus, setListStatus] = useState([]);
  const [listContract, setListContract] = useState([]);
<<<<<<< HEAD

=======
>>>>>>> 103083c6cb18e0f69c01e255502e2c9c68ee3a6f
  useEffect(() => {
    SetIsLoading(true);

    (async () => {
      let getListCustomer = await getData(
        `Customer/GetListCustomerOptionSelect`
      );
<<<<<<< HEAD
=======

>>>>>>> 103083c6cb18e0f69c01e255502e2c9c68ee3a6f
      if (getListCustomer && getListCustomer.length > 0) {
        let obj = [];
        obj.push({
          value: "",
<<<<<<< HEAD
          label: "Chọn khách hàng",
=======
          label: "Chọn cung đường",
>>>>>>> 103083c6cb18e0f69c01e255502e2c9c68ee3a6f
        });
        getListCustomer.map((val) => {
          obj.push({
            value: val.maKh,
            label: val.maKh + " - " + val.tenKh,
          });
        });
        setListCustomer(obj);
      }

      let getListDVT = await getData("Common/GetListDVT");
      let getListVehicleType = await getData("Common/GetListVehicleType");
      let getListGoodsType = await getData("Common/GetListGoodsType");
      let getListTransportType = await getData("Common/GetListTransportType");
      let getListStatus = await getData(
        "Common/GetListStatus?statusType=common"
      );

      setListVehicleType(getListVehicleType);
      setListGoodsType(getListGoodsType);
      setListDVT(getListDVT);
      setListTransportType(getListTransportType);
      setListStatus(getListStatus);
    })();

    SetIsLoading(false);
  }, []);

  const handleOnchangeListCustomer = (val) => {
<<<<<<< HEAD
    SetIsLoading(true);

    setListContract([]);
    setListRoad([]);
=======
>>>>>>> 103083c6cb18e0f69c01e255502e2c9c68ee3a6f
    setValue("MaKh", val);
    setValue("MaHopDong", { value: "", label: "Chọn Hợp Đồng" });
    setValue("MaCungDuong", { value: "", label: "Chọn Cung Đường" });
    getListRoadAndContract(val.value);
<<<<<<< HEAD

    SetIsLoading(false);
=======
>>>>>>> 103083c6cb18e0f69c01e255502e2c9c68ee3a6f
  };

  const getListRoadAndContract = async (MaKh) => {
    let getListRoad = await getData(
      `Road/GetListRoadOptionSelect?MaKH=${MaKh}`
    );
    if (getListRoad && getListRoad.length > 0) {
      let obj = [];
      obj.push({
        value: "",
        label: "Chọn cung đường",
      });
      getListRoad.map((val) => {
        obj.push({
          value: val.maCungDuong,
          label: val.maCungDuong + " - " + val.tenCungDuong,
        });
      });
      setListRoad(obj);
    }

    let getListContract = await getData(
      `Contract/GetListContractSelect?MaKH=${MaKh}`
    );

    if (getListContract && getListContract.length > 0) {
      let obj = [];
      obj.push({
        value: "",
        label: "Chọn Hợp Đồng",
      });
      getListContract.map((val) => {
        obj.push({
          value: val.maHopDong,
          label: val.maHopDong + " - " + val.tenHienThi,
        });
      });
      setListContract(obj);
    }
  };

  const handleResetClick = () => {
    reset();
    setValue("MaCungDuong", { value: "", label: "Chọn cung đường" });
    setValue("MaKh", { value: "", label: "Chọn Khách Hàng" });
<<<<<<< HEAD
    setValue("MaHopDong", { value: "", label: "Chọn Hợp Đồng" });
=======
>>>>>>> 103083c6cb18e0f69c01e255502e2c9c68ee3a6f
  };

  const onSubmit = async (data, e) => {
    SetIsLoading(true);

    const createPriceTable = await postData("PriceTable/CreatePriceTable", {
      maBangGia: data.MaBangGia,
      maHopDong: data.MaHopDong.value,
      maKh: data.MaKh.value,
      maCungDuong: data.MaCungDuong.value,
      maLoaiPhuongTien: data.MaLoaiPhuongTien,
      giaVnd: data.GiaVND,
      giaUsd: data.GiaUSD,
      maDvt: data.MaDVT,
      soLuong: data.SoLuong,
      maLoaiHangHoa: data.MaLoaiHangHoa,
      maPtvc: data.MaPTVC,
      ngayApDung: data.NgayApDung,
      trangThai: data.TrangThai,
    });

    if (createPriceTable === 1) {
      getListPriceTable(1);
      reset();
    }

    SetIsLoading(false);
  };
  return (
    <>
      <div className="card card-primary">
        <div className="card-header">
          <h3 className="card-title">Form Thêm Mới Bảng Giá</h3>
        </div>
        <div>{IsLoading === true && <div>Loading...</div>}</div>

        {IsLoading === false && (
          <form onSubmit={handleSubmit(onSubmit)}>
            <div className="card-body">
              <div className="row">
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="NgayApDung">Ngày Áp Dụng</label>
                    <div className="input-group ">
                      <Controller
                        control={control}
                        name="NgayApDung"
                        render={({ field }) => (
                          <DatePicker
                            className="form-control"
                            dateFormat="dd/MM/yyyy"
                            onChange={(date) => field.onChange(date)}
                            selected={field.value}
                          />
                        )}
                        rules={Validate.NgayApDung}
                      />
                      {errors.NgayApDung && (
                        <span className="text-danger">
                          {errors.NgayApDung.message}
                        </span>
                      )}
                    </div>
                  </div>
                </div>
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="MaBangGia">Mã bảng giá</label>
                    <input
                      type="text"
                      className="form-control"
                      id="MaBangGia"
                      {...register("MaBangGia", Validate.MaBangGia)}
                    />
                    {errors.MaBangGia && (
                      <span className="text-danger">
                        {errors.MaBangGia.message}
                      </span>
                    )}
                  </div>
                </div>
              </div>
<<<<<<< HEAD
=======

>>>>>>> 103083c6cb18e0f69c01e255502e2c9c68ee3a6f
              <div className="row">
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="KhachHang">Khách Hàng</label>
                    <Controller
                      name="MaKh"
                      control={control}
                      render={({ field }) => (
                        <Select
                          {...field}
                          classNamePrefix={"form-control"}
                          onChange={(field) =>
                            handleOnchangeListCustomer(field)
                          }
                          value={field.value}
                          options={listCustomer}
                          defaultValue={{ value: "", label: "Chọn Khách Hàng" }}
                        />
                      )}
                      rules={Validate.MaKh}
                    />
                    {errors.MaKh && (
                      <span className="text-danger">{errors.MaKh.message}</span>
                    )}
                  </div>
                </div>
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="MaHopDong">Hợp Đồng</label>
                    <Controller
                      name="MaHopDong"
                      control={control}
                      render={({ field }) => (
                        <Select
                          {...field}
                          classNamePrefix={"form-control"}
                          value={field.value}
                          options={listContract}
                          defaultValue={{ value: "", label: "Chọn hợp đồng" }}
                        />
                      )}
                      rules={Validate.MaHopDong}
                    />
                    {errors.MaHopDong && (
                      <span className="text-danger">
                        {errors.MaHopDong.message}
                      </span>
                    )}
                  </div>
                </div>
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="MaCungDuong">Cung Đường</label>
                    <Controller
                      name="MaCungDuong"
                      control={control}
                      render={({ field }) => (
                        <Select
                          {...field}
                          classNamePrefix={"form-control"}
                          value={field.value}
                          options={listRoad}
                          defaultValue={{ value: "", label: "Chọn cung đường" }}
                        />
                      )}
                      rules={Validate.MaCungDuong}
                    />
                    {errors.MaCungDuong && (
                      <span className="text-danger">
                        {errors.MaCungDuong.message}
                      </span>
                    )}
                  </div>
                </div>
              </div>
<<<<<<< HEAD
=======
              <div className="row"></div>
>>>>>>> 103083c6cb18e0f69c01e255502e2c9c68ee3a6f
              <div className="row">
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="VND">Giá VND</label>
                    <input
                      type="text"
                      className="form-control"
                      id="VND"
                      {...register("GiaVND", Validate.GiaUSD)}
                    />
                    {errors.GiaVND && (
                      <span className="text-danger">
                        {errors.GiaVND.message}
                      </span>
                    )}
                  </div>
                </div>
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="USD">Giá USD</label>
                    <input
                      type="text"
                      className="form-control"
                      id="USD"
                      {...register("GiaUSD", Validate.GiaUSD)}
                    />
                    {errors.GiaUSD && (
                      <span className="text-danger">
                        {errors.GiaUSD.message}
                      </span>
                    )}
                  </div>
                </div>
              </div>
<<<<<<< HEAD
=======

>>>>>>> 103083c6cb18e0f69c01e255502e2c9c68ee3a6f
              <div className="row">
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="MaDVT">Đơn vị tính</label>
                    <select
                      className="form-control"
                      {...register("MaDVT", Validate.MaDVT)}
                    >
                      <option value="">Chọn đơn vị tính</option>
                      {listDVT &&
                        listDVT.map((val) => {
                          return (
                            <option value={val.maDvt} key={val.maDvt}>
                              {val.tenDvt}
                            </option>
                          );
                        })}
                    </select>
                    {errors.MaDVT && (
                      <span className="text-danger">
                        {errors.MaDVT.message}
                      </span>
                    )}
                  </div>
                </div>
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="SoLuong">Số lượng</label>
                    <input
                      type="text"
                      className="form-control"
                      id="SoLuong"
                      {...register("SoLuong", Validate.SoLuong)}
                    />
                    {errors.SoLuong && (
                      <span className="text-danger">
                        {errors.SoLuong.message}
                      </span>
                    )}
                  </div>
                </div>
              </div>
              <div className="row">
                <div className="col col-sm">
                  {" "}
                  <div className="form-group">
                    <label htmlFor="PTVC">Phương thức vận chuyển</label>
                    <select
                      className="form-control"
                      {...register("MaPTVC", Validate.MaPTVC)}
                    >
                      <option value="">Chọn phương thức vận chuyển</option>
                      {listTransportType &&
                        listTransportType.map((val) => {
                          return (
                            <option value={val.maPtvc} key={val.maPtvc}>
                              {val.tenPtvc}
                            </option>
                          );
                        })}
                    </select>
                    {errors.MaPTVC && (
                      <span className="text-danger">
                        {errors.MaPTVC.message}
                      </span>
                    )}
                  </div>
                </div>
                <div className="col col-sm">
                  {" "}
                  <div className="form-group">
                    <label htmlFor="MaLoaiPhuongTien">Loại phương tiện</label>
                    <select
                      className="form-control"
                      {...register(
                        "MaLoaiPhuongTien",
                        Validate.MaLoaiPhuongTien
                      )}
                    >
                      <option value="">Chọn loại phương tiện</option>
                      {listVehicleType &&
                        listVehicleType.map((val) => {
                          return (
                            <option
                              value={val.maLoaiPhuongTien}
                              key={val.maLoaiPhuongTien}
                            >
                              {val.tenLoaiPhuongTien}
                            </option>
                          );
                        })}
                    </select>
                    {errors.MaLoaiPhuongTien && (
                      <span className="text-danger">
                        {errors.MaLoaiPhuongTien.message}
                      </span>
                    )}
                  </div>
                </div>
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="MaLoaiHangHoa">Loại Hàng Hóa</label>
                    <select
                      className="form-control"
                      {...register("MaLoaiHangHoa", Validate.MaLoaiHangHoa)}
                    >
                      <option value="">Chọn loại hàng hóa</option>
                      {listGoodsType &&
                        listGoodsType.map((val) => {
                          return (
                            <option
                              value={val.maLoaiHangHoa}
                              key={val.maLoaiHangHoa}
                            >
                              {val.tenLoaiHangHoa}
                            </option>
                          );
                        })}
                    </select>
                    {errors.MaLoaiHangHoa && (
                      <span className="text-danger">
                        {errors.MaLoaiHangHoa.message}
                      </span>
                    )}
                  </div>
                </div>
              </div>
              <div className="form-group">
                <label htmlFor="TrangTHai">Trạng Thái</label>
                <select
                  className="form-control"
                  {...register("TrangThai", Validate.TrangThai)}
                >
                  <option value="">Chọn Trạng Thái</option>
                  {listStatus &&
                    listStatus.map((val) => {
                      return (
                        <option value={val.maTrangThai} key={val.maTrangThai}>
                          {val.tenTrangThai}
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
    </>
  );
};

export default AddPriceTable;
