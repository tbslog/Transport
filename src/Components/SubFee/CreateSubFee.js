import { useState, useEffect, useMemo } from "react";
import { getData, postData } from "../Common/FuncAxios";
import { useForm, Controller } from "react-hook-form";
import Select from "react-select";
import LoadingPage from "../Common/Loading/LoadingPage";

const CreateSubFee = (props) => {
  const { getListSubFee } = props;

  const {
    register,
    reset,
    setValue,
    watch,
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
    MaHopDong: {
      required: "Không được để trống",
    },
    MaKh: {
      required: "Không được để trống",
    },
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
  const [listCustomerType, setListCustomerType] = useState([]);
  const [listSubFee, setListSubFee] = useState([]);
  const [listSubFeeSelect, setListSubFeeSelect] = useState([]);
  const [listVehicleType, setlistVehicleType] = useState([]);

  const [listFPlace, setListFPlace] = useState([]);
  const [listSPlace, setListSPlace] = useState([]);
  const [listEPlace, setListEPlace] = useState([]);

  const [listAccountCus, setListAccountCus] = useState([]);

  useEffect(() => {
    (async () => {
      SetIsLoading(true);
      let getListVehicleType = await getData("Common/GetListVehicleType");
      setlistVehicleType(getListVehicleType);
      let getListCustommerType = await getData(`Common/GetListCustommerType`);
      setListCustomerType(getListCustommerType);
      let getListGoodsType = await getData("Common/GetListGoodsType");
      setListGoodsTypes(getListGoodsType);
      let getListSubFee = await getData(`SubFeePrice/GetListSubFeeSelect`);

      const getListPlace = await getData(
        "Address/GetListAddressSelect?pointType=&type="
      );

      let arrPlace = [];
      arrPlace.push({ label: "-- Rỗng --", value: null });

      getListPlace.forEach((val) => {
        arrPlace.push({
          label: val.tenDiaDiem + " - " + val.loaiDiaDiem,
          value: val.maDiaDiem,
        });
      });

      setListFPlace(arrPlace);
      setListSPlace(arrPlace);
      setListEPlace(arrPlace);

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

  useEffect(() => {
    if (listCustomerType && listCustomerType.length > 0) {
      handleOnChangeContractType("KH");
    }
  }, [listCustomerType]);

  const handleOnchangeListCustomer = async (val) => {
    setListContract([]);
    setValue("MaKh", val);
    setValue("MaHopDong", null);
    setValue("CungDuong", null);
    if (val && val.value) {
      await getListContract(val.value);
    }
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
  };

  const handleOnchangeListSubFee = async (val) => {
    setValue("LoaiPhuPhi", val);
    var des = listSubFee.filter((x) => x.subFeeId === val.value)[0];
    setValue("SubFeeDes", des.subFeeDescription);
  };

  const handleOnChangeCustomer = async (val) => {
    if (val && Object.keys(val).length > 0) {
      setValue("MaKH", val);
      const getListAcc = await getData(
        `AccountCustomer/GetListAccountSelectByCus?cusId=${val.value}`
      );
      if (getListAcc && getListAcc.length > 0) {
        var obj = [];
        obj.push({ label: "-- Để Trống --", value: null });
        getListAcc.map((val) => {
          obj.push({
            value: val.accountId,
            label: val.accountId + " - " + val.accountName,
          });
        });
        setListAccountCus(obj);
      } else {
        setListAccountCus([]);
      }
    }
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
      CusType: data.PhanLoaiDoiTac,
      accountId: !data.AccountCus ? null : data.AccountCus.value,
      ContractId: data.MaHopDong.value,
      SfId: data.LoaiPhuPhi.value,
      GoodsType: !data.MaLoaiHangHoa ? null : data.MaLoaiHangHoa,
      firstPlace: !data.DiemDau
        ? null
        : !data.DiemDau.value
        ? null
        : data.DiemDau.value,
      secondPlace: !data.DiemCuoi
        ? null
        : !data.DiemCuoi.value
        ? null
        : data.DiemCuoi.value,
      getEmptyPlace: !data.DiemLayTraRong
        ? null
        : !data.DiemLayTraRong.value
        ? null
        : data.DiemLayTraRong.value,
      VehicleType: !data.LoaiPhuongTien ? null : data.LoaiPhuongTien,
      Price: data.DonGia,
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
                  <label htmlFor="PhanLoaiDoiTac">Phân Loại Đối Tác(*)</label>
                  <select
                    className="form-control"
                    {...register("PhanLoaiDoiTac", Validate.PhanLoaiDoiTac)}
                    onChange={(e) => handleOnChangeContractType(e.target.value)}
                  >
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
                        onChange={(field) => {
                          handleOnchangeListCustomer(field);
                          handleOnChangeCustomer(field);
                        }}
                      />
                    )}
                    rules={Validate.MaKh}
                  />
                  {errors.MaKh && (
                    <span className="text-danger">{errors.MaKh.message}</span>
                  )}
                </div>
              </div>
              {watch("PhanLoaiDoiTac") && watch("PhanLoaiDoiTac") === "KH" && (
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="AccountCus">Account</label>
                    <Controller
                      name="AccountCus"
                      control={control}
                      render={({ field }) => (
                        <Select
                          {...field}
                          classNamePrefix={"form-control"}
                          value={field.value}
                          options={listAccountCus}
                        />
                      )}
                    />
                    {errors.AccountCus && (
                      <span className="text-danger">
                        {errors.AccountCus.message}
                      </span>
                    )}
                  </div>
                </div>
              )}

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
                <div className="col-sm">
                  <div className="form-group">
                    <label htmlFor="DiemLayTraRong">Điểm Lấy/Trả Rỗng</label>
                    <Controller
                      name={`DiemLayTraRong`}
                      control={control}
                      render={({ field }) => (
                        <Select
                          {...field}
                          classNamePrefix={"form-control"}
                          value={field.value}
                          options={listEPlace}
                          placeholder={"Điểm Lấy/Trả Rỗng"}
                        />
                      )}
                    />
                    {errors.DiemLayTraRong && (
                      <span className="text-danger">
                        {errors.DiemLayTraRong.message}
                      </span>
                    )}
                  </div>
                </div>
              </div>
              <div className="col col-sm">
                <div className="form-group">
                  <label htmlFor="DiemDau">Điểm Đóng Hàng</label>
                  <Controller
                    name={`DiemDau`}
                    control={control}
                    render={({ field }) => (
                      <Select
                        {...field}
                        classNamePrefix={"form-control"}
                        value={field.value}
                        options={listFPlace}
                        placeholder={"Điểm Đóng Hàng"}
                      />
                    )}
                    // rules={{
                    //   required: "không được để trống",
                    //   validate: (value) => {
                    //     if (!value.value) {
                    //       return "không được để trống";
                    //     }
                    //   },
                    // }}
                  />
                  {errors.DiemDau && (
                    <span className="text-danger">
                      {errors.DiemDau.message}
                    </span>
                  )}
                </div>
              </div>
              <div className="col col-sm">
                <div className="col-sm">
                  <div className="form-group">
                    <label htmlFor="DiemCuoi">Điểm Hạ Hàng</label>
                    <Controller
                      name={`DiemCuoi`}
                      control={control}
                      render={({ field }) => (
                        <Select
                          {...field}
                          classNamePrefix={"form-control"}
                          value={field.value}
                          options={listSPlace}
                          placeholder={"Điểm Hạ Hàng"}
                        />
                      )}
                      // rules={{
                      //   required: "không được để trống",
                      //   validate: (value) => {
                      //     if (!value.value) {
                      //       return "không được để trống";
                      //     }
                      //   },
                      // }}
                    />
                    {errors.DiemCuoi && (
                      <span className="text-danger">
                        {errors.DiemCuoi.message}
                      </span>
                    )}
                  </div>
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
                  <label htmlFor="LoaiPhuongTien">Loại Phương Tiện</label>
                  <select
                    className="form-control"
                    {...register(`LoaiPhuongTien`, Validate.LoaiPhuongTien)}
                  >
                    <option value="">-- Để Trống --</option>
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
                  {errors.LoaiPhuongTien && (
                    <span className="text-danger">
                      {errors.LoaiPhuongTien.message}
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
