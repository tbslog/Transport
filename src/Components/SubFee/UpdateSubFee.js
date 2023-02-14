import { useState, useEffect, useMemo } from "react";
import { getData, postData } from "../Common/FuncAxios";
import { useForm, Controller } from "react-hook-form";
import Select from "react-select";

const UpdateSubFee = (props) => {
  const { getListSubFee, selectIdClick, hideModal } = props;

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
  const [listArea, setListArea] = useState([]);
  const [listRoad, setListRoad] = useState([]);

  useEffect(() => {
    (async () => {
      SetIsLoading(true);
      let getListCustommerType = await getData(`Common/GetListCustommerType`);
      setListCustomerType(getListCustommerType);
      let getListGoodsType = await getData("Common/GetListGoodsType");
      setListGoodsTypes(getListGoodsType);

      const getListArea = await getData("Common/GetListArea");
      setListArea(getListArea);

      let getListRoad = await getData(`Road/GetListRoadOptionSelect`);
      if (getListRoad && getListRoad.length > 0) {
        let obj = [];
        obj.push({ value: "", label: "-- Để Trống --" });
        getListRoad.map((val) => {
          obj.push({
            value: val.maCungDuong,
            label: val.tenCungDuong,
          });
        });
        setListRoad(obj);
      }

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

  useEffect(() => {
    if (
      selectIdClick &&
      Object.keys(selectIdClick).length > 0 &&
      listCustomerType &&
      listGoodTypes &&
      listSubFee &&
      listSubFeeSelect &&
      listCustomerType.length > 0 &&
      listGoodTypes.length > 0 &&
      listSubFee.length > 0 &&
      listSubFeeSelect.length > 0
    ) {
      SetIsLoading(true);
      setValue("PhanLoaiDoiTac", selectIdClick.customerType);
      handleOnChangeContractType(selectIdClick.customerType);
    }
  }, [
    selectIdClick,
    listCustomerType,
    listGoodTypes,
    listSubFee,
    listSubFeeSelect,
  ]);

  useEffect(() => {
    if (
      listCustomer &&
      listCustomer.length > 0 &&
      selectIdClick &&
      Object.keys(selectIdClick).length > 0
    ) {
      setValue(
        "MaKh",
        {
          ...listCustomer.filter((x) => x.value === selectIdClick.customerId),
        }[0]
      );
      getListContract(selectIdClick.customerId);
    }
  }, [listCustomer, selectIdClick]);

  useEffect(() => {
    if (
      selectIdClick &&
      Object.keys(selectIdClick).length > 0 &&
      listContract &&
      listContract.length > 0
    ) {
      setValue(
        "LoaiPhuPhi",
        {
          ...listSubFeeSelect.filter((x) => x.value === selectIdClick.sfId),
        }[0]
      );

      var des = listSubFee.filter((x) => x.subFeeId === selectIdClick.sfId)[0];
      setValue("SubFeeDes", des.subFeeDescription);
      setValue("KhuVuc", selectIdClick.areaID);
      setValue("MaLoaiHangHoa", selectIdClick.goodsType);
      setValue(
        "CungDuong",
        {
          ...listRoad.filter((x) => x.value === selectIdClick.tripID),
        }[0]
      );

      setValue("DonGia", selectIdClick.unitPrice);
      setValue("Description", selectIdClick.description);

      setValue(
        "MaHopDong",
        {
          ...listContract.filter((x) => x.value === selectIdClick.contractId),
        }[0]
      );

      SetIsLoading(false);
    }
  }, [selectIdClick, listContract]);

  const handleOnchangeListCustomer = async (val) => {
    SetIsLoading(true);

    setListContract([]);
    setValue("MaKh", val);
    setValue("MaHopDong", null);

    if (val && val.value) {
      await getListContract(val.value);
    }

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
        obj.push({ value: "", label: "Tất Cả" });
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

    const createSubFreePrice = await postData(
      `SubFeePrice/UpdateSubFeePrice?Id=${selectIdClick.priceId}`,
      {
        CusType: data.PhanLoaiDoiTac,
        ContractId: !data.MaHopDong ? null : data.MaHopDong.value,
        SfId: data.LoaiPhuPhi.value,
        GoodsType: !data.MaLoaiHangHoa ? null : data.MaLoaiHangHoa,
        AreaId: !data.KhuVuc ? null : data.KhuVuc,
        TripId: !data.CungDuong
          ? null
          : !data.CungDuong.value
          ? null
          : data.CungDuong.value,
        Price: data.DonGia,
        Description: !data.Description ? "" : data.Description,
      }
    );

    if (createSubFreePrice === 1) {
      getListSubFee(1);
      hideModal();
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
              <div className="col-sm">
                <div className="form-group">
                  <label htmlFor="KhuVuc">Khu Vực</label>
                  <select
                    className="form-control"
                    {...register("KhuVuc", Validate.KhuVuc)}
                  >
                    <option value="">-- Bỏ Trống --</option>
                    {listArea &&
                      listArea.length > 0 &&
                      listArea.map((val, index) => {
                        return (
                          <option value={val.id} key={index}>
                            {val.tenKhuVuc}
                          </option>
                        );
                      })}
                  </select>
                  {errors.KhuVuc && (
                    <span className="text-danger">{errors.KhuVuc.message}</span>
                  )}
                </div>
              </div>
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

export default UpdateSubFee;
