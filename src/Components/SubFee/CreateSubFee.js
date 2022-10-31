import { useState, useEffect, useMemo } from "react";
import { getData, postData } from "../Common/FuncAxios";
import { useForm, Controller } from "react-hook-form";
import DatePicker from "react-datepicker";
import moment from "moment";
import Select from "react-select";

const CreateSubFee = (props) => {
  const {} = props;

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
    MaSoXe: {
      required: "Không được để trống",
      maxLength: {
        value: 10,
        message: "Không được vượt quá 10 ký tự",
      },
      minLength: {
        value: 6,
        message: "Không được ít hơn 6 ký tự",
      },
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt",
      },
    },
    TrongTaiToiDa: {
      maxLength: {
        value: 10,
        message: "Không được vượt quá 12 ký tự",
      },
      minLength: {
        value: 1,
        message: "Không được ít hơn 1 ký tự",
      },
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[0-9]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt",
      },
    },
    TrongTaiToiThieu: {
      maxLength: {
        value: 10,
        message: "Không được vượt quá 12 ký tự",
      },
      minLength: {
        value: 1,
        message: "Không được ít hơn 1 ký tự",
      },
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[0-9]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt",
      },
    },
    TGKhauHao: {
      maxLength: {
        value: 10,
        message: "Không được vượt quá 12 ký tự",
      },
      minLength: {
        value: 1,
        message: "Không được ít hơn 1 ký tự",
      },
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[0-9]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt",
      },
    },
    LoaiXe: {
      required: "Không được bỏ trống",
    },
    MaGPS: {
      required: "Không được bỏ trống",
    },
    MaGPSMobile: {
      required: "Không được bỏ trống",
    },
    TrangThai: {
      required: "Không được bỏ trống",
    },
  };
  const [IsLoading, SetIsLoading] = useState(true);
  const [listContract, setListContract] = useState([]);
  const [listCustomer, setListCustomer] = useState([]);
  const [listGoodTypes, setListGoodsTypes] = useState([]);
  const [listPlace, setListPlace] = useState([]);
  const [listCustomerType, setListCustomerType] = useState([]);
  const [listRoad, setListRoad] = useState([]);
  const [listSubFee, setListSubFee] = useState([]);
  const [listSubFeeSelect, setListSubFeeSelect] = useState([]);

  useEffect(() => {
    (async () => {
      SetIsLoading(true);
      let getListCustommerType = await getData(`Common/GetListCustommerType`);
      setListCustomerType(getListCustommerType);
      let getListGoodsType = await getData("Common/GetListGoodsType");
      setListGoodsTypes(getListGoodsType);
      let getListSubFee = await getData(`SubFeePrice/GetListSubFeeSelect`);

      if (getListSubFee && getListSubFee.length > 0) {
        let obj = [];
        getListSubFee.map((val) => {
          obj.push({
            value: val.subFeeId,
            label: val.subFeeId + " - " + val.subFeeName,
          });
        });
        setListSubFee(getListSubFee);
        setListSubFeeSelect(obj);
      }
      SetIsLoading(false);
    })();
  }, []);

  const handleOnchangeListCustomer = (val) => {
    SetIsLoading(true);

    setListContract([]);
    setListRoad([]);
    setValue("MaKh", val);
    setValue("MaHopDong", null);
    setValue("CungDuong", null);
    getListContract(val.value);

    SetIsLoading(false);
  };

  const handleOnChangeContractType = async (val) => {
    handleResetClick();
    setValue("PhanLoaiDoiTac", val);
    if (val && val !== "") {
      let getListCustomer = await getData(
        `Customer/GetListCustomerOptionSelect`
      );
      if (getListCustomer && getListCustomer.length > 0) {
        getListCustomer = getListCustomer.filter((x) => x.loaiKH === val);
        let obj = [];

        getListCustomer.map((val) => {
          obj.push({
            value: val.maKh,
            label: val.maKh + " - " + val.tenKh,
          });
        });
        setListCustomer(obj);
      }
    } else {
      handleResetClick();
    }
  };

  const getListContract = async (CusId) => {
    SetIsLoading(true);

    let getListContract = await getData(
      `Contract/GetListContractSelect?MaKH=${CusId}`
    );

    if (getListContract && getListContract.length > 0) {
      let obj = [];

      getListContract.map((val) => {
        obj.push({
          value: val.maHopDong,
          label: val.maHopDong + " - " + val.tenHienThi,
        });
      });
      setListContract(obj);
    }
    SetIsLoading(false);
  };

  const handleOnChangeContract = async (val) => {
    SetIsLoading(true);
    setValue(
      "MaHopDong",
      { ...listContract.filter((x) => x.value === val.value) }[0]
    );
    let getListRoad = await getData(
      `Road/GetListRoadOptionSelect?ContractId=${val.value}`
    );
    if (getListRoad && getListRoad.length > 0) {
      let obj = [];
      obj.push({ label: "-- Bỏ Trống --", val: "" });
      getListRoad.map((val) => {
        obj.push({
          value: val.maCungDuong,
          label: val.maCungDuong + " - " + val.tenCungDuong,
        });
      });
      setListRoad(obj);
    }
    SetIsLoading(false);
  };

  const handleOnchangeListSubFee = async (val) => {
    var des = listSubFee.filter((x) => x.subFeeId === val.value)[0];
    setValue("SubFeeDes", des.subFeeDescription);
  };

  const handleResetClick = () => {
    reset();
    setValue("MaKh", null);
    setValue("MaHopDong", null);
    setValue("CungDuong", null);
    setListRoad([]);
    setListContract([]);
    setListRoad([]);
    setListCustomer([]);
  };

  const onSubmit = async (data, e) => {
    SetIsLoading(true);

    // let arr = [];
    // data.optionRoad.map((val) => {
    //   arr.push({
    //     maHopDong: data.MaHopDong.value,
    //     maKh: data.MaKh.value,
    //     maPtvc: val.MaPTVC,
    //     maCungDuong: val.MaCungDuong.value,
    //     maLoaiPhuongTien: val.MaLoaiPhuongTien,
    //     maLoaiDoiTac: data.PhanLoaiDoiTac,
    //     donGia: val.DonGia,
    //     maDvt: val.MaDVT,
    //     maLoaiHangHoa: val.MaLoaiHangHoa,
    //     ngayHetHieuLuc: !val.NgayHetHieuLuc
    //       ? null
    //       : moment(new Date(val.NgayHetHieuLuc).toISOString()).format(
    //           "YYYY-MM-DD"
    //         ),
    //   });
    // });

    // const createPriceTable = await postData("PriceTable/CreatePriceTable", arr);
    // if (createPriceTable === 1) {
    //   reset();
    // }
    SetIsLoading(false);
  };

  return (
    <div className="card card-primary">
      <div className="card-header">
        <h3 className="card-title">Form Thêm Mới Phụ Phí</h3>
      </div>
      <div>{IsLoading === true && <div>Loading...</div>}</div>

      {IsLoading === false && (
        <form onSubmit={handleSubmit(onSubmit)}>
          <div className="card-body">
            <div className="row">
              <div className="col col-sm">
                <div className="form-group">
                  <label htmlFor="PhanLoaiDoiTac">Phân Loại Đối Tác</label>
                  <select
                    className="form-control"
                    {...register("PhanLoaiDoiTac", Validate.PhanLoaiDoiTac)}
                    onChange={(e) => handleOnChangeContractType(e.target.value)}
                  >
                    <option value="">Chọn phân loại đối tác</option>
                    {listCustomerType &&
                      listCustomerType.map((val) => {
                        return (
                          <option value={val.maLoaiKh} key={val.maLoaiKh}>
                            {val.tenLoaiKh}
                          </option>
                        );
                      })}
                  </select>
                  {errors.PhanLoaiDoiTac && (
                    <span className="text-danger">
                      {errors.PhanLoaiDoiTac.message}
                    </span>
                  )}
                </div>
              </div>
              <div className="col col-sm">
                <div className="form-group">
                  <label htmlFor="KhachHang">Khách Hàng / Nhà Cung Cấp</label>
                  <Controller
                    name="MaKh"
                    control={control}
                    render={({ field }) => (
                      <Select
                        {...field}
                        classNamePrefix={"form-control"}
                        value={field.value}
                        options={listCustomer}
                        onChange={(field) => handleOnchangeListCustomer(field)}
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
                    rules={Validate.MaHopDong}
                    control={control}
                    render={({ field }) => (
                      <Select
                        {...field}
                        classNamePrefix={"form-control"}
                        value={field.value}
                        options={listContract}
                        onChange={(field) => handleOnChangeContract(field)}
                      />
                    )}
                  />
                  {errors.MaHopDong && (
                    <span className="text-danger">
                      {errors.MaHopDong.message}
                    </span>
                  )}
                </div>
              </div>
            </div>
            <div className="row">
              <div className="col col-sm">
                <div className="form-group">
                  <label htmlFor="CungDuong">Cung Đường</label>
                  <Controller
                    name="CungDuong"
                    control={control}
                    render={({ field }) => (
                      <Select
                        {...field}
                        classNamePrefix={"form-control"}
                        value={field.value}
                        options={listRoad}
                      />
                    )}
                    rules={Validate.CungDuong}
                  />
                  {errors.CungDuong && (
                    <span className="text-danger">
                      {errors.CungDuong.message}
                    </span>
                  )}
                </div>
              </div>
              <div className="col col-sm">
                <div className="form-group">
                  <label htmlFor="MaLoaiHangHoa">Loại Hàng Hóa</label>
                  <select
                    className="form-control"
                    {...register(`MaLoaiHangHoa`, Validate.MaLoaiHangHoa)}
                  >
                    <option value="">-- Để Trống --</option>
                    {listGoodTypes &&
                      listGoodTypes.map((val) => {
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
              <div className="col col-sm">
                <div className="form-group">
                  <label htmlFor="LoaiPhuPhi">Loại Phụ Phí</label>
                  <Controller
                    name="LoaiPhuPhi"
                    control={control}
                    render={({ field }) => (
                      <Select
                        {...field}
                        classNamePrefix={"form-control"}
                        value={field.value}
                        options={listSubFeeSelect}
                        onChange={(field) => handleOnchangeListSubFee(field)}
                      />
                    )}
                    rules={Validate.LoaiPhuPhi}
                  />
                  {errors.LoaiPhuPhi && (
                    <span className="text-danger">
                      {errors.LoaiPhuPhi.message}
                    </span>
                  )}
                </div>
              </div>
            </div>
            <div className="row">
              <div className="col col-sm">
                <label htmlFor="SubFeeDes">Mô Tả Loại Phụ Phí</label>
                <div className="form-group">
                  <textarea
                    disabled={true}
                    className="form-control"
                    type="text"
                    id="SubFeeDes"
                    rows={5}
                    {...register(`SubFeeDes`, Validate.SubFeeDes)}
                  ></textarea>
                  {errors.SubFeeDes && (
                    <span className="text-danger">
                      {errors.SubFeeDes.message}
                    </span>
                  )}
                </div>
              </div>
            </div>
            <div>
              <div className="col col-sm">
                <div className="form-group">
                  <label htmlFor="MaLoaiHangHoa">Đơn Giá</label>
                  <input
                    type="text"
                    className="form-control"
                    id="DonGia"
                    {...register(`DonGia`, Validate.DonGia)}
                  />
                  {errors.DonGia && (
                    <span className="text-danger">{errors.DonGia.message}</span>
                  )}
                </div>
              </div>
            </div>
            <div>
              <div className="col col-sm">
                <div className="form-group">
                  <label htmlFor="ThongTinMoTa">Thông Tin Mô Tả</label>
                  <textarea
                    className="form-control"
                    type="text"
                    id="ThongTinMoTa"
                    rows={5}
                    {...register(`ThongTinMoTa`, Validate.ThongTinMoTa)}
                  ></textarea>
                  {errors.ThongTinMoTa && (
                    <span className="text-danger">
                      {errors.ThongTinMoTa.message}
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

export default CreateSubFee;
