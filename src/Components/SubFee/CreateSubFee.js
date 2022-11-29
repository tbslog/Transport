import { useState, useEffect, useMemo } from "react";
import { getData, postData } from "../Common/FuncAxios";
import { useForm, Controller } from "react-hook-form";
import Select from "react-select";

const CreateSubFee = (props) => {
  const { getListSubFee } = props;

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
    PhanLoaiDoiTac: {
      required: "Không được để trống",
    },
    MaKh: { required: "Không được để trống" },
    MaHopDong: { required: "Không được để trống" },
    DiemDau: {},
    DiemCuoi: {},
    MaLoaiHangHoa: {},
    LoaiPhuPhi: {
      required: "Không được để trống",
    },
    DonGia: {
      required: "Không được để trống",
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
  };
  const [IsLoading, SetIsLoading] = useState(true);
  const [listContract, setListContract] = useState([]);
  const [listCustomer, setListCustomer] = useState([]);
  const [listGoodTypes, setListGoodsTypes] = useState([]);
  const [listPlace, setListPlace] = useState([]);
  const [listCustomerType, setListCustomerType] = useState([]);
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

      let getListPlace = await getData(`Address/GetListAddressSelect`);

      if (getListPlace && getListPlace.length > 0) {
        let obj = [];
        obj.push({ value: null, label: "-- Để Trống --" });
        getListPlace.map((val) => {
          obj.push({
            value: val.maDiaDiem,
            label: val.maDiaDiem + " - " + val.tenDiaDiem,
          });
        });
        setListPlace(obj);
      }

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
      `Contract/GetListContractSelect?MaKH=${CusId}&getProductService=true`
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

  const handleOnchangeListSubFee = async (val) => {
    setValue("LoaiPhuPhi", val);
    var des = listSubFee.filter((x) => x.subFeeId === val.value)[0];
    setValue("SubFeeDes", des.subFeeDescription);
  };

  const handleResetClick = () => {
    reset();
    setValue("MaKh", null);
    setValue("MaHopDong", null);
    setValue("CungDuong", null);
    setValue("LoaiPhuPhi", null);

    setListContract([]);
    setListCustomer([]);
  };

  const onSubmit = async (data) => {
    SetIsLoading(true);

    const createSubFreePrice = await postData("SubFeePrice/CreateSubFeePrice", {
      ContractId: data.MaHopDong.value,
      SfId: data.LoaiPhuPhi.value,
      GoodsType: !data.MaLoaiHangHoa ? null : data.MaLoaiHangHoa,
      FirstPlace: !data.DiemDau ? null : data.DiemDau.value,
      SecondPlace: !data.DiemCuoi ? null : data.DiemCuoi.value,
      UnitPrice: data.DonGia,
      Description: !data.Description ? "" : data.Description,
    });

    if (createSubFreePrice === 1) {
      getListSubFee(1);
      reset();
    }
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
                  <label htmlFor="PhanLoaiDoiTac">Phân Loại Đối Tác(*)</label>
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
                  <label htmlFor="KhachHang">
                    Khách Hàng / Nhà Cung Cấp(*)
                  </label>
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
                  <label htmlFor="MaHopDong">Hợp Đồng(*)</label>
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
                  <label htmlFor="DiemDau">Điểm 1</label>
                  <Controller
                    name="DiemDau"
                    control={control}
                    render={({ field }) => (
                      <Select
                        {...field}
                        classNamePrefix={"form-control"}
                        value={field.value}
                        options={listPlace}
                      />
                    )}
                    rules={Validate.DiemDau}
                  />
                  {errors.DiemDau && (
                    <span className="text-danger">
                      {errors.DiemDau.message}
                    </span>
                  )}
                </div>
              </div>
              <div className="col col-sm">
                <div className="form-group">
                  <label htmlFor="DiemCuoi">Điểm 2</label>
                  <Controller
                    name="DiemCuoi"
                    control={control}
                    render={({ field }) => (
                      <Select
                        {...field}
                        classNamePrefix={"form-control"}
                        value={field.value}
                        options={listPlace}
                      />
                    )}
                    rules={Validate.DiemCuoi}
                  />
                  {errors.DiemCuoi && (
                    <span className="text-danger">
                      {errors.DiemCuoi.DiemCuoi}
                    </span>
                  )}
                </div>
              </div>
            </div>
            <div className="row">
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
                  <label htmlFor="LoaiPhuPhi">Loại Phụ Phí(*)</label>
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
              <div className="col col-sm">
                <div className="form-group">
                  <label htmlFor="MaLoaiHangHoa">Đơn Giá(*)</label>
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
