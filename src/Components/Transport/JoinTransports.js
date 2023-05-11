import { useState, useEffect, useLayoutEffect } from "react";
import { getData, getDataCustom, postData } from "../Common/FuncAxios";
import { useForm, Controller, useFieldArray } from "react-hook-form";
import DatePicker from "react-datepicker";
import Select from "react-select";
import moment from "moment";
import { ToastError, ToastWarning } from "../Common/FuncToast";
import LoadingPage from "../Common/Loading/LoadingPage";
import Cookies from "js-cookie";

const JoinTransports = (props) => {
  const { items, clearItems, hideModal, getListTransport, selectIdClick } =
    props;
  const accountType = Cookies.get("AccType");
  const [IsLoading, SetIsLoading] = useState(false);
  const {
    register,
    reset,
    setValue,
    control,
    watch,
    formState: { errors },
    handleSubmit,
  } = useForm({
    mode: "onChange",
  });

  const { fields, remove } = useFieldArray({
    control, // control props comes from useForm (optional: if you are using FormContext)
    name: "listTransport", // unique name for your Field Array
  });

  const Validate = {
    PTVanChuyen: { required: "Không được để trống" },
    DiemLayTraRong: { required: "Không được để trống" },
    NhaCungCap: { required: "Không được để trống" },
    XeVanChuyen: { required: "Không được để trống" },
    LoaiHangHoa: {
      required: "Không được để trống",
    },
    ThuTuGiaoHang: {
      maxLength: {
        value: 2,
        message: "Không được vượt quá 2 ký tự",
      },
      pattern: {
        value: /^[0-9]*$/,
        message: "Chỉ được nhập ký tự là số",
      },
    },
    CONTNO: {
      pattern: {
        value: /([A-Z]{3})([UJZ])(\d{6})(\d)/,
        message: "Mã không không đúng, vui lòng viết hoa",
      },
    },
  };

  const [listPoint, setListPoint] = useState([]);
  const [listDriver, setListDriver] = useState([]);
  const [listRomooc, setListRomooc] = useState([]);
  const [listSupplier, setListSupplier] = useState([]);
  const [listGoodsType, setListGoodsType] = useState([]);

  const [listVehicleType, setlistVehicleType] = useState([]);
  const [listVehicle, setListVehicle] = useState([]);
  const [listVehicleTypeSelect, setlistVehicleTypeSelect] = useState([]);
  const [listVehicleSelect, setListVehicleSelect] = useState([]);

  const [dataHandling, setDataHandling] = useState({});

  const [totalCBM, setTotalCBM] = useState("");
  const [totalWGT, setTotalWGT] = useState("");

  useEffect(() => {
    (async () => {
      SetIsLoading(true);
      let getListVehicleType = await getData("Common/GetListVehicleType");
      setlistVehicleTypeSelect(getListVehicleType);
      setlistVehicleType(getListVehicleType);

      let getListGoodsType = await getData("Common/GetListGoodsType");
      setListGoodsType(getListGoodsType);

      const getListPoint = await getData(
        "Address/GetListAddressSelect?pointType=&type=Diem"
      );
      if (getListPoint && getListPoint.length > 0) {
        var obj = [];
        getListPoint.map((val) => {
          obj.push({
            value: val.maDiaDiem,
            label: val.maDiaDiem + " - " + val.tenDiaDiem,
          });
        });
        setListPoint(obj);
      }

      let getListNCC = await getData(`Customer/GetListCustomerOptionSelect`);
      if (getListNCC && getListNCC.length > 0) {
        let listSup = getListNCC.filter((x) => x.loaiKH === "NCC");
        let objSup = [];

        listSup.map((val) => {
          objSup.push({
            value: val.maKh,
            label: val.maKh + " - " + val.tenKh,
          });
        });
        setListSupplier(objSup);
      }

      let getListDriver = await getData(`Driver/GetListSelectDriver`);
      if (getListDriver && getListDriver.length > 0) {
        let obj = [];
        getListDriver.map((val) => {
          obj.push({
            value: val.maTaiXe,
            label: val.maTaiXe + " - " + val.hoVaTen,
          });
        });
        setListDriver(obj);
      }

      let getRomooc = await getData(`Romooc/GetListRomoocSelect`);
      if (getRomooc && getRomooc.length > 0) {
        let obj = [];
        obj.push({ label: "--Rỗng--", value: null });
        getRomooc.map((val) => {
          obj.push({
            value: val.maRomooc,
            label: val.maRomooc + " - " + val.romoocType,
          });
        });
        setListRomooc(obj);
      }

      let listVehicle = await getData("Vehicle/GetListVehicleSelect");
      if (listVehicle && listVehicle.length > 0) {
        let arr = [];
        listVehicle.map((val) => {
          arr.push({
            label: val.text,
            value: val.vehicleId,
          });
        });
        setListVehicle(arr);
      }
      SetIsLoading(false);
    })();
  }, []);

  useLayoutEffect(() => {
    if (
      items &&
      items.length > 0 &&
      listDriver &&
      listRomooc &&
      listVehicleType &&
      listVehicleType.length > 0
    ) {
      (async () => {
        resetForm();
        SetIsLoading(true);

        let arrTransport = [];
        items.map((val) => {
          arrTransport.push(val.maVanDon);
        });

        let listVehicle = await getData("Vehicle/GetListVehicleSelect");
        if (listVehicle && listVehicle.length > 0) {
          let arr = [];
          listVehicle.map((val) => {
            arr.push({
              label: val.text,
              value: val.vehicleId,
            });
          });
          setListVehicle(arr);
          setListVehicleSelect(arr);
        }

        if (arrTransport && arrTransport.length > 0) {
          let dataTransports = await getDataCustom(
            `BillOfLading/LoadJoinTransports`,
            { TransportIds: arrTransport }
          );
          let data = dataTransports.loadTransports;

          if (data && data.length > 0) {
            setValue("listTransport", data);
            for (let i = 0; i <= data.length - 1; i++) {
              setValue(
                `listTransport.${i}.ThuTuGiaoHang`,
                data[i].thuTuGiaoHang
              );
              setValue(`listTransport.${i}.HandlingId`, data[i].handlingId);
              setValue(`listTransport.${i}.LoaiVanDon`, data[i].loaiVanDon);
              setValue(`listTransport.${i}.MaPTVC`, data[i].maPTVC);
              setValue(
                `listTransport.${i}.LoaiHangHoa`,
                data[i].loaiHangHoa == null ? "Normal" : data[i].loaiHangHoa
              );
              setValue(`listTransport.${i}.MaVanDon`, data[i].maVanDon);
              setValue(`listTransport.${i}.MaVanDonKH`, data[i].maVanDonKH);
              setValue(`listTransport.${i}.MaKH`, data[i].maKH);
              setValue(`listTransport.${i}.AccountName`, data[i].accountName);
              setValue(`listTransport.${i}.DiemDau`, data[i].diemDau);
              setValue(`listTransport.${i}.DiemCuoi`, data[i].diemCuoi);
              setValue(`listTransport.${i}.KhoiLuong`, data[i].tongKhoiLuong);
              setValue(`listTransport.${i}.SoKien`, data[i].tongSoKien);
              setValue(`listTransport.${i}.TheTich`, data[i].tongTheTich);
              setValue(`listTransport.${i}.SealHQ`, data[i].sealHq);
              setValue(`listTransport.${i}.SealNP`, data[i].sealNp);
              setValue(
                `listTransport.${i}.ContNo`,
                data[i].contNo === null ? null : data[i].contNo
              );
              setValue(
                `listTransport.${i}.DiemLayRong`,
                data[i].diemLayRong === null
                  ? null
                  : {
                      ...listPoint.filter(
                        (x) => x.value === data[i].diemLayRong
                      ),
                    }[0]
              );
              setValue(
                `listTransport.${i}.DiemTraRong`,
                data[i].diemTraRong === null
                  ? null
                  : {
                      ...listPoint.filter(
                        (x) => x.value === data[i].diemTraRong
                      ),
                    }[0]
              );
              setValue(
                `listTransport.${i}.TGHanLenh`,
                data[i].tgHanLenh === null ? null : new Date(data[i].tgHanLenh)
              );
              setValue(
                `listTransport.${i}.TGLayRong`,
                data[i].tgLayRong === null ? null : new Date(data[i].tgLayRong)
              );
              setValue(
                `listTransport.${i}.TGTraRong`,
                data[i].tgTraRong === null ? null : new Date(data[i].tgTraRong)
              );
              setValue(
                `listTransport.${i}.TGHaCang`,
                data[i].tgHaCang === null ? null : new Date(data[i].tgHaCang)
              );
              setValue(
                `listTransport.${i}.ThoiGianLayHang`,
                data[i].thoiGianLayHang === null
                  ? null
                  : new Date(data[i].thoiGianLayHang)
              );
              setValue(
                `listTransport.${i}.ThoiGianTraHang`,
                data[i].thoiGianTraHang === null
                  ? null
                  : new Date(data[i].thoiGianTraHang)
              );
            }
          }
        }
      })();
    }
    SetIsLoading(false);
  }, [items, listVehicleType, listDriver, listRomooc]);

  useLayoutEffect(() => {
    if (
      selectIdClick &&
      listDriver &&
      listRomooc &&
      listVehicleType &&
      Object.keys(selectIdClick).length > 0 &&
      listVehicleType.length > 0
    ) {
      (async () => {
        resetForm();
        SetIsLoading(true);

        let listVehicle = await getData("Vehicle/GetListVehicleSelect");
        if (listVehicle && listVehicle.length > 0) {
          let arr = [];
          listVehicle.map((val) => {
            arr.push({
              label: val.text,
              value: val.vehicleId,
            });
          });
          setListVehicle(arr);
          setListVehicleSelect(arr);
        }

        let dataTransports = await getDataCustom(
          `BillOfLading/LoadJoinTransports`,
          { TransportIds: [], MaChuyen: selectIdClick.maChuyen }
        );
        let dataHandling = dataTransports.handlingLess;

        setDataHandling(dataHandling);
        setValue("PTVanChuyen", dataHandling.ptVanChuyen);

        setValue(
          "NhaCungCap",
          {
            ...listSupplier.filter((x) => x.value === dataHandling.donViVanTai),
          }[0]
        );

        setValue(
          "TaiXe",
          { ...listDriver.filter((x) => x.value === dataHandling.taiXe) }[0]
        );
        setValue(
          "Romooc",
          !dataHandling.romooc
            ? null
            : {
                ...listRomooc.filter((x) => x.value === dataHandling.romooc),
              }[0]
        );

        let data = dataTransports.loadTransports;
        if (data && data.length > 0) {
          setValue("listTransport", data);
          for (let i = 0; i <= data.length - 1; i++) {
            setValue(`listTransport.${i}.ThuTuGiaoHang`, data[i].thuTuGiaoHang);
            setValue(`listTransport.${i}.HandlingId`, data[i].handlingId);
            setValue(`listTransport.${i}.LoaiVanDon`, data[i].loaiVanDon);
            setValue(`listTransport.${i}.MaPTVC`, data[i].maPTVC);
            setValue(
              `listTransport.${i}.LoaiHangHoa`,
              data[i].loaiHangHoa == null ? "Normal" : data[i].loaiHangHoa
            );
            setValue(`listTransport.${i}.MaVanDon`, data[i].maVanDon);
            setValue(`listTransport.${i}.MaVanDonKH`, data[i].maVanDonKH);
            setValue(`listTransport.${i}.MaKH`, data[i].maKH);
            setValue(`listTransport.${i}.AccountName`, data[i].accountName);
            setValue(`listTransport.${i}.DiemDau`, data[i].diemDau);
            setValue(`listTransport.${i}.DiemCuoi`, data[i].diemCuoi);
            setValue(`listTransport.${i}.KhoiLuong`, data[i].tongKhoiLuong);
            setValue(`listTransport.${i}.TheTich`, data[i].tongTheTich);
            setValue(`listTransport.${i}.SoKien`, data[i].tongSoKien);
            setValue(`listTransport.${i}.SealHQ`, data[i].sealHq);
            setValue(`listTransport.${i}.SealNP`, data[i].sealNp);
            setValue(
              `listTransport.${i}.ContNo`,
              data[i].contNo === null ? null : data[i].contNo
            );
            setValue(
              `listTransport.${i}.DiemLayRong`,
              data[i].diemLayRong === null
                ? null
                : {
                    ...listPoint.filter((x) => x.value === data[i].diemLayRong),
                  }[0]
            );
            setValue(
              `listTransport.${i}.DiemTraRong`,
              data[i].diemTraRong === null
                ? null
                : {
                    ...listPoint.filter((x) => x.value === data[i].diemTraRong),
                  }[0]
            );
            setValue(
              `listTransport.${i}.TGHanLenh`,
              data[i].tgHanLenh === null ? null : new Date(data[i].tgHanLenh)
            );
            setValue(
              `listTransport.${i}.TGLayRong`,
              data[i].tgLayRong === null ? null : new Date(data[i].tgLayRong)
            );
            setValue(
              `listTransport.${i}.TGTraRong`,
              data[i].tgTraRong === null ? null : new Date(data[i].tgTraRong)
            );
            setValue(
              `listTransport.${i}.TGHaCang`,
              data[i].tgHaCang === null ? null : new Date(data[i].tgHaCang)
            );
            setValue(
              `listTransport.${i}.ThoiGianLayHang`,
              data[i].thoiGianLayHang === null
                ? null
                : new Date(data[i].thoiGianLayHang)
            );
            setValue(
              `listTransport.${i}.ThoiGianTraHang`,
              data[i].thoiGianTraHang === null
                ? null
                : new Date(data[i].thoiGianTraHang)
            );
          }
        }
        SetIsLoading(false);
      })();
    }
  }, [selectIdClick, listVehicleType, listDriver, listRomooc]);

  useEffect(() => {
    if (
      selectIdClick &&
      listVehicle &&
      dataHandling &&
      Object.keys(dataHandling).length > 0 &&
      Object.keys(selectIdClick).length > 0
    ) {
      handleOnChangeWeight(watch("PTVanChuyen"));
      setValue(
        "XeVanChuyen",
        {
          ...listVehicle.filter((x) => x.value === dataHandling.xeVanChuyen),
        }[0]
      );
    }
  }, [selectIdClick, listVehicle, dataHandling]);

  useLayoutEffect(() => {
    if (watch("MaPTVC")) {
      let arrVehicleType = listVehicleType;
      let arrVehicle = listVehicle;

      if (watch("MaPTVC") === "LTL") {
        arrVehicleType = arrVehicleType.filter((x) =>
          x.maLoaiPhuongTien.includes("TRUCK")
        );
        arrVehicle = arrVehicle.filter((x) => x.label.includes("TRUCK"));
      }
      if (watch("MaPTVC") === "LCL") {
        arrVehicleType = arrVehicleType.filter((x) =>
          x.maLoaiPhuongTien.includes("CONT")
        );
        arrVehicle = arrVehicle.filter((x) => x.label.includes("CONT"));
      }
      setlistVehicleTypeSelect(arrVehicleType);
      setListVehicleSelect(arrVehicle);
    }
  }, [watch("MaPTVC")]);

  const onSubmit = async (data) => {
    SetIsLoading(true);

    let arrTransports = [];
    data.listTransport.map((val) => {
      arrTransports.push({
        HandlingId: val.HandlingId,
        ThuTuGiaoHang: !val.ThuTuGiaoHang ? null : val.ThuTuGiaoHang,
        MaVanDon: val.maVanDon,
        MaLoaiHangHoa: val.LoaiHangHoa,
        MaDVT: "CHUYEN",
        ContNo: val.ContNo,
        DiemTraRong: !val.DiemTraRong ? null : val.DiemTraRong.value,
        DiemLayRong: !val.DiemLayRong ? null : val.DiemLayRong.value,
        SealHQ: val.SealHQ,
        SealNP: val.SealNP,
        TGLayHang: !val.ThoiGianLayHang
          ? null
          : moment(new Date(val.ThoiGianLayHang).toISOString()).format(
              "yyyy-MM-DDTHH:mm:ss.SSS"
            ),
        TGTraHang: !val.ThoiGianTraHang
          ? null
          : moment(new Date(val.ThoiGianTraHang).toISOString()).format(
              "yyyy-MM-DDTHH:mm:ss.SSS"
            ),
        TGTraRong: !val.TGTraRong
          ? null
          : moment(new Date(val.TGTraRong).toISOString()).format(
              "yyyy-MM-DDTHH:mm:ss.SSS"
            ),
        TGLayRong: !val.TGLayRong
          ? null
          : moment(new Date(val.TGLayRong).toISOString()).format(
              "yyyy-MM-DDTHH:mm:ss.SSS"
            ),
        TGHanLenh: !val.TGHanLenh
          ? null
          : moment(new Date(val.TGHanLenh).toISOString()).format(
              "yyyy-MM-DDTHH:mm:ss.SSS"
            ),
        TGHaCang: !val.TGHaCang
          ? null
          : moment(new Date(val.TGHaCang).toISOString()).format(
              "yyyy-MM-DDTHH:mm:ss.SSS"
            ),
      });
    });

    // if (arrTransports.length < 1) {
    //ToastError("Vui lòng chọn đơn để ghép chuyến");
    //SetIsLoading(false);
    //   return;
    // }

    const dataJoin = {
      arrTransports: arrTransports,
      PTVanChuyen: data.PTVanChuyen,
      DonViVanTai: data.NhaCungCap.value,
      XeVanChuyen: !data.XeVanChuyen ? null : data.XeVanChuyen.value,
      TaiXe: !data.TaiXe ? null : data.TaiXe.value,
      GhiChu: data.GhiChu,
      Romooc: !data.Romooc ? null : data.Romooc.value,
    };

    if (items && items.length > 0) {
      const create = await postData(
        "BillOfLading/CreateHandlingLess",
        dataJoin
      );
      if (create === 1) {
        clearItems();
        getListTransport();
        hideModal();
      }
    }
    if (selectIdClick && Object.keys(selectIdClick).length > 0) {
      const update = await postData(
        `BillOfLading/UpdateHandlingLess?handlingId=${selectIdClick.maChuyen}`,
        dataJoin
      );
      if (update === 1) {
        clearItems();
        getListTransport();
        hideModal();
      }
    }

    SetIsLoading(false);
  };

  const handleOnChangeWeight = async (vehicleType) => {
    if (vehicleType) {
      let tongkl = 0;
      let tongth = 0;

      let data = watch("listTransport");

      if (data && data.length > 0) {
        data.forEach((val) => {
          if (val.maPTVC === "LCL" || val.maPTVC === "LTL") {
            if (val.KhoiLuong) {
              tongkl = tongkl + val.KhoiLuong;
            }
            if (val.TheTich) {
              tongth = tongth + val.TheTich;
            }
          }
        });

        let getTonnageVehicle = await getData(
          `BillOfLading/LayTrongTaiXe?vehicleType=${vehicleType}`
        );

        if (getTonnageVehicle) {
          let vehicleCBM = getTonnageVehicle.cbm;
          let vehicleWGT = getTonnageVehicle.wgt;
          setTotalWGT({ tongkl, vehicleWGT });
          setTotalCBM({ tongth, vehicleCBM });
        } else {
          setTotalWGT({});
          setTotalCBM({});
        }
      }
    }
  };

  const resetForm = () => {
    reset();
    setValue("NhaCungCap", null);
    setValue("DiemLayTraRong", null);
    setValue("XeVanChuyen", null);
    setValue("TaiXe", null);
  };

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
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="PTVanChuyen">
                      Phương tiện vận chuyển(*)
                    </label>
                    <select
                      className="form-control"
                      {...register(`PTVanChuyen`, Validate.PTVanChuyen)}
                      disabled={
                        accountType && accountType === "NV" ? false : true
                      }
                      onChange={(e) => handleOnChangeWeight(e.target.value)}
                    >
                      <option value="">Chọn phương Tiện Vận Chuyển</option>
                      {listVehicleTypeSelect &&
                        listVehicleTypeSelect.map((val) => {
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
                    {errors.PTVanChuyen && (
                      <span className="text-danger">
                        {errors.PTVanChuyen.message}
                      </span>
                    )}
                  </div>
                </div>

                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="NhaCungCap">Đơn Vị Vận Tải(*)</label>
                    <Controller
                      name={`NhaCungCap`}
                      control={control}
                      render={({ field }) => (
                        <Select
                          {...field}
                          isDisabled={
                            accountType && accountType === "NV" ? false : true
                          }
                          classNamePrefix={"form-control"}
                          value={field.value}
                          options={listSupplier}
                        />
                      )}
                      rules={{
                        required: "không được để trống",
                      }}
                    />
                    {errors.NhaCungCap && (
                      <span className="text-danger">
                        {errors.NhaCungCap.message}
                      </span>
                    )}
                  </div>
                </div>
              </div>

              <div className="row">
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="XeVanChuyen">Xe Vận Chuyển(*)</label>
                    <Controller
                      name={`XeVanChuyen`}
                      control={control}
                      render={({ field }) => (
                        <Select
                          {...field}
                          classNamePrefix={"form-control"}
                          value={field.value}
                          options={listVehicleSelect}
                        />
                      )}
                    />
                    {errors.XeVanChuyen && (
                      <span className="text-danger">
                        {errors.XeVanChuyen.message}
                      </span>
                    )}
                  </div>
                </div>
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="TaiXe">Tài Xế(*)</label>
                    <Controller
                      name={`TaiXe`}
                      control={control}
                      render={({ field }) => (
                        <Select
                          {...field}
                          classNamePrefix={"form-control"}
                          value={field.value}
                          options={listDriver}
                        />
                      )}
                    />
                    {errors.TaiXe && (
                      <span className="text-danger">
                        {errors.TaiXe.message}
                      </span>
                    )}
                  </div>
                </div>
                {watch(`listTransport.0.MaPTVC`) &&
                  (watch(`listTransport.0.MaPTVC`).includes("LCL") ||
                    watch(`listTransport.0.MaPTVC`).includes("FCL")) && (
                    <>
                      <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="Romooc">Số Romooc</label>
                          <Controller
                            name={`Romooc`}
                            control={control}
                            render={({ field }) => (
                              <Select
                                {...field}
                                classNamePrefix={"form-control"}
                                value={field.value}
                                options={listRomooc}
                              />
                            )}
                          />
                          {errors.Romooc && (
                            <span className="text-danger">
                              {errors.Romooc.message}
                            </span>
                          )}
                        </div>
                      </div>
                    </>
                  )}
              </div>
              <div className="row">
                <div className="col col-12">
                  <div className="form-group">
                    <label htmlFor="GhiChu">Ghi Chú</label>
                    <div className="input-group ">
                      <textarea
                        className="form-control"
                        rows={3}
                        {...register(`GhiChu`)}
                      ></textarea>
                    </div>
                  </div>
                </div>
              </div>
              <div className="row">
                <div className="col col-sm">
                  <p style={{ fontWeight: "bold" }}>
                    Tổng Số Trọng Lượng :
                    {totalWGT &&
                      totalWGT.tongkl > 0 &&
                      totalWGT.vehicleWGT > 0 && (
                        <>
                          {totalWGT.tongkl + "/" + totalWGT.vehicleWGT}
                          {totalWGT.vehicleWGT < totalWGT.tongkl ? (
                            <>
                              <p style={{ color: "red" }}>
                                (Vượt quá tổng trọng lượng của Xe)
                              </p>
                            </>
                          ) : (
                            <></>
                          )}
                        </>
                      )}
                  </p>
                </div>
                <div className="col col-sm">
                  <p style={{ fontWeight: "bold" }}>
                    Tổng Số Thể Tích :
                    {totalCBM &&
                      totalCBM.vehicleCBM > 0 &&
                      totalCBM.tongth > 0 && (
                        <>
                          {totalCBM.tongth + "/" + totalCBM.vehicleCBM}
                          {totalCBM.vehicleCBM < totalCBM.tongth ? (
                            <>
                              <p style={{ color: "red" }}>
                                (Vượt quá tổng thể tích của Xe)
                              </p>
                            </>
                          ) : (
                            <></>
                          )}
                        </>
                      )}
                  </p>
                </div>
              </div>
              <div className="row">
                <div className="table-responsive" style={{ overflow: "auto" }}>
                  <table
                    className="table table-bordered "
                    style={{
                      whiteSpace: "nowrap",
                      width: "220%",
                      height: "500px",
                    }}
                  >
                    <thead>
                      <tr>
                        <th>Thứ Tự Giao Hàng</th>
                        <th>
                          <div>Loại Vận Đơn</div>
                        </th>
                        <th style={{ width: "6%" }}>
                          <div>Booking No</div>
                        </th>
                        {accountType && accountType === "NV" && (
                          <>
                            <th style={{ width: "6%" }}>
                              <div>Khách Hàng</div>
                            </th>
                            <th style={{ width: "6%" }}>
                              <div>Account</div>
                            </th>
                          </>
                        )}
                        <th style={{ width: "8%" }}>
                          <div>Loại Hàng Hóa</div>
                        </th>
                        <th style={{ width: "3%" }}>
                          <div>PTVC</div>
                        </th>
                        <th style={{ width: "8%" }}>
                          <div>Điểm Đóng Hàng</div>
                        </th>
                        <th style={{ width: "8%" }}>
                          <div>Điểm Hạ Hàng</div>
                        </th>

                        {(watch(`listTransport.0.MaPTVC`) === "FCL" ||
                          watch(`listTransport.0.MaPTVC`) === "LCL") && (
                          <>
                            <th style={{ width: "8%" }}>
                              <div>Điểm Lấy/Trả Rỗng</div>
                            </th>
                            <th style={{ width: "5%" }}>
                              <div>Mã ContNo</div>
                            </th>
                            <th style={{ width: "10%" }}>
                              <div>TG Hạn Lệnh/CUT OFF</div>
                            </th>
                            <th style={{ width: "10%" }}>
                              <div>TG Lấy/Trả Rỗng</div>
                            </th>
                          </>
                        )}
                        <th>
                          <div>SEAL NP</div>
                        </th>
                        <th>
                          <div>SEAL HQ</div>
                        </th>
                        <th style={{ width: "10%" }}>
                          <div>TG Đóng Hàng</div>
                        </th>
                        <th style={{ width: "10%" }}>
                          <div>TG Hạ Hàng</div>
                        </th>

                        <th>
                          <div>Khối Lượng</div>
                        </th>
                        <th>
                          <div>Thể Tích</div>
                        </th>
                        <th>
                          <div>Số Kiện</div>
                        </th>
                        <th></th>
                      </tr>
                    </thead>
                    <tbody>
                      {fields.map((value, index) => (
                        <tr key={index}>
                          <td>
                            <input
                              autoComplete="false"
                              type="text"
                              className="form-control"
                              id="ThuTuGiaoHang"
                              {...register(
                                `listTransport.${index}.ThuTuGiaoHang`,
                                Validate.ThuTuGiaoHang
                              )}
                              disabled={
                                accountType && accountType === "NV"
                                  ? false
                                  : true
                              }
                            />
                            {errors.listTransport?.[index]?.ThuTuGiaoHang && (
                              <span className="text-danger">
                                {
                                  errors.listTransport?.[index]?.ThuTuGiaoHang
                                    .message
                                }
                              </span>
                            )}
                          </td>
                          <td>
                            <div hidden={true}>
                              <div className="form-group">
                                <input
                                  readOnly={true}
                                  type="text"
                                  className="form-control"
                                  id="MaVanDon"
                                  {...register(
                                    `listTransport.${index}.MaVanDon`
                                  )}
                                />
                              </div>
                            </div>
                            <input
                              readOnly={true}
                              autoComplete="false"
                              type="text"
                              className="form-control"
                              id="TransportType"
                              {...register(`listTransport.${index}.LoaiVanDon`)}
                            />
                          </td>
                          <td>
                            <div className="form-group">
                              <input
                                readOnly={true}
                                type="text"
                                className="form-control"
                                id="MaVanDonKH"
                                {...register(
                                  `listTransport.${index}.MaVanDonKH`
                                )}
                              />
                            </div>
                          </td>
                          {accountType && accountType === "NV" && (
                            <>
                              <td>
                                <div className="form-group">
                                  <input
                                    readOnly={true}
                                    type="text"
                                    className="form-control"
                                    id="MaKH"
                                    {...register(`listTransport.${index}.MaKH`)}
                                  />
                                </div>
                              </td>
                              <td>
                                <div className="form-group">
                                  <input
                                    readOnly={true}
                                    type="text"
                                    className="form-control"
                                    id="AccountName"
                                    {...register(
                                      `listTransport.${index}.AccountName`
                                    )}
                                  />
                                </div>
                              </td>
                            </>
                          )}

                          <td>
                            <div className="col col-sm">
                              <div className="form-group">
                                <select
                                  className="form-control"
                                  {...register(
                                    `listTransport.${index}.LoaiHangHoa`,
                                    Validate.LoaiHangHoa
                                  )}
                                  disabled={
                                    accountType && accountType === "NV"
                                      ? false
                                      : true
                                  }
                                >
                                  <option value="">Chọn Loại Hàng Hóa</option>
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
                                {errors.listTransport?.[index]?.LoaiHangHoa && (
                                  <span className="text-danger">
                                    {
                                      errors.listTransport?.[index]?.LoaiHangHoa
                                        .message
                                    }
                                  </span>
                                )}
                              </div>
                            </div>
                          </td>
                          <td>
                            <input
                              readOnly={true}
                              autoComplete="false"
                              type="text"
                              className="form-control"
                              id="TransportType"
                              {...register(`listTransport.${index}.MaPTVC`)}
                            />
                          </td>
                          <td>
                            <div className="form-group">
                              <input
                                readOnly={true}
                                type="text"
                                className="form-control"
                                id="DiemDau"
                                {...register(`listTransport.${index}.DiemDau`)}
                              />
                            </div>
                          </td>
                          <td>
                            <div className="form-group">
                              <input
                                readOnly={true}
                                type="text"
                                className="form-control"
                                id="DiemCuoi"
                                {...register(`listTransport.${index}.DiemCuoi`)}
                              />
                            </div>
                          </td>
                          {(watch(`listTransport.${index}.MaPTVC`) === "FCL" ||
                            watch(`listTransport.${index}.MaPTVC`) ===
                              "LCL") && (
                            <>
                              {watch(`listTransport.${index}.LoaiVanDon`) ===
                                "nhap" && (
                                <>
                                  <td>
                                    <Controller
                                      name={`listTransport.${index}.DiemTraRong`}
                                      control={control}
                                      render={({ field }) => (
                                        <Select
                                          {...field}
                                          isDisabled={
                                            accountType && accountType === "NV"
                                              ? false
                                              : true
                                          }
                                          classNamePrefix={"form-control"}
                                          value={field.value}
                                          options={listPoint}
                                        />
                                      )}
                                      rules={{
                                        required: "không được để trống",
                                        validate: (value) => {
                                          if (!value.value) {
                                            return "không được để trống";
                                          }
                                        },
                                      }}
                                    />
                                    {errors.listTransport?.[index]
                                      ?.DiemTraRong && (
                                      <span className="text-danger">
                                        {
                                          errors.listTransport?.[index]
                                            ?.DiemTraRong.message
                                        }
                                      </span>
                                    )}
                                  </td>
                                  <td>
                                    <div className="form-group">
                                      <input
                                        type="text"
                                        className="form-control"
                                        id="ContNo"
                                        {...register(
                                          `listTransport.${index}.ContNo`
                                        )}
                                      />
                                    </div>
                                  </td>
                                  <td>
                                    <div className="input-group ">
                                      <Controller
                                        control={control}
                                        name={`listTransport.${index}.TGHanLenh`}
                                        render={({ field }) => (
                                          <DatePicker
                                            disabled={
                                              accountType &&
                                              accountType === "NV"
                                                ? false
                                                : true
                                            }
                                            className="form-control"
                                            showTimeSelect
                                            timeFormat="HH:mm"
                                            dateFormat="dd/MM/yyyy HH:mm"
                                            onChange={(date) =>
                                              field.onChange(date)
                                            }
                                            selected={field.value}
                                          />
                                        )}
                                        rules={{
                                          required: "không được để trống",
                                        }}
                                      />
                                      {errors.listTransport?.[index]
                                        ?.TGHanLenh && (
                                        <span className="text-danger">
                                          {
                                            errors.listTransport?.[index]
                                              ?.TGHanLenh.message
                                          }
                                        </span>
                                      )}
                                    </div>
                                  </td>
                                  <td>
                                    <div className="input-group ">
                                      <Controller
                                        control={control}
                                        name={`listTransport.${index}.TGTraRong`}
                                        render={({ field }) => (
                                          <DatePicker
                                            disabled={
                                              accountType &&
                                              accountType === "NV"
                                                ? false
                                                : true
                                            }
                                            className="form-control"
                                            showTimeSelect
                                            timeFormat="HH:mm"
                                            dateFormat="dd/MM/yyyy HH:mm"
                                            onChange={(date) =>
                                              field.onChange(date)
                                            }
                                            selected={field.value}
                                          />
                                        )}
                                      />
                                      {errors.listTransport?.[index]
                                        ?.TGTraRong && (
                                        <span className="text-danger">
                                          {
                                            errors.listTransport?.[index]
                                              ?.TGTraRong.message
                                          }
                                        </span>
                                      )}
                                    </div>
                                  </td>
                                </>
                              )}
                              {watch(`listTransport.${index}.LoaiVanDon`) ===
                                "xuat" && (
                                <>
                                  <td>
                                    <Controller
                                      name={`listTransport.${index}.DiemLayRong`}
                                      control={control}
                                      render={({ field }) => (
                                        <Select
                                          {...field}
                                          isDisabled={
                                            accountType && accountType === "NV"
                                              ? false
                                              : true
                                          }
                                          classNamePrefix={"form-control"}
                                          value={field.value}
                                          options={listPoint}
                                        />
                                      )}
                                      rules={{
                                        required: "không được để trống",
                                        validate: (value) => {
                                          if (!value.value) {
                                            return "không được để trống";
                                          }
                                        },
                                      }}
                                    />
                                    {errors.listTransport?.[index]
                                      ?.DiemLayRong && (
                                      <span className="text-danger">
                                        {
                                          errors.listTransport?.[index]
                                            ?.DiemLayRong.message
                                        }
                                      </span>
                                    )}
                                  </td>
                                  <td>
                                    <div className="form-group">
                                      <input
                                        type="text"
                                        className="form-control"
                                        id="ContNo"
                                        {...register(
                                          `listTransport.${index}.ContNo`
                                        )}
                                      />
                                    </div>
                                  </td>
                                  <td>
                                    <div className="input-group ">
                                      <Controller
                                        control={control}
                                        name={`listTransport.${index}.TGHaCang`}
                                        render={({ field }) => (
                                          <DatePicker
                                            disabled={
                                              accountType &&
                                              accountType === "NV"
                                                ? false
                                                : true
                                            }
                                            className="form-control"
                                            showTimeSelect
                                            timeFormat="HH:mm"
                                            dateFormat="dd/MM/yyyy HH:mm"
                                            onChange={(date) =>
                                              field.onChange(date)
                                            }
                                            selected={field.value}
                                          />
                                        )}
                                        rules={{
                                          required: "không được để trống",
                                        }}
                                      />
                                      {errors.listTransport?.[index]
                                        ?.TGHaCang && (
                                        <span className="text-danger">
                                          {
                                            errors.listTransport?.[index]
                                              ?.TGHaCang.message
                                          }
                                        </span>
                                      )}
                                    </div>
                                  </td>
                                  <td>
                                    <div className="input-group ">
                                      <Controller
                                        control={control}
                                        name={`listTransport.${index}.TGLayRong`}
                                        render={({ field }) => (
                                          <DatePicker
                                            disabled={
                                              accountType &&
                                              accountType === "NV"
                                                ? false
                                                : true
                                            }
                                            className="form-control"
                                            showTimeSelect
                                            timeFormat="HH:mm"
                                            dateFormat="dd/MM/yyyy HH:mm"
                                            onChange={(date) =>
                                              field.onChange(date)
                                            }
                                            selected={field.value}
                                          />
                                        )}
                                      />
                                      {errors.listTransport?.[index]
                                        ?.TGLayRong && (
                                        <span className="text-danger">
                                          {
                                            errors.listTransport?.[index]
                                              ?.TGLayRong.message
                                          }
                                        </span>
                                      )}
                                    </div>
                                  </td>
                                </>
                              )}
                            </>
                          )}

                          <td>
                            <div className="form-group">
                              <input
                                type="text"
                                className="form-control"
                                id="SealNP"
                                {...register(`listTransport.${index}.SealNP`)}
                              />
                            </div>
                          </td>
                          <td>
                            <div className="form-group">
                              <input
                                type="text"
                                className="form-control"
                                id="SealHQ  "
                                {...register(`listTransport.${index}.SealHQ`)}
                              />
                            </div>
                          </td>
                          <td>
                            <div className="input-group ">
                              <Controller
                                control={control}
                                name={`listTransport.${index}.ThoiGianLayHang`}
                                render={({ field }) => (
                                  <DatePicker
                                    disabled={
                                      accountType && accountType === "NV"
                                        ? false
                                        : true
                                    }
                                    className="form-control"
                                    showTimeSelect
                                    timeFormat="HH:mm"
                                    dateFormat="dd/MM/yyyy HH:mm"
                                    onChange={(date) => field.onChange(date)}
                                    selected={field.value}
                                  />
                                )}
                              />
                              {errors.listTransport?.[index]
                                ?.ThoiGianLayHang && (
                                <span className="text-danger">
                                  {
                                    errors.listTransport?.[index]
                                      ?.ThoiGianLayHang.message
                                  }
                                </span>
                              )}
                            </div>
                          </td>
                          <td>
                            <div className="input-group ">
                              <Controller
                                control={control}
                                name={`listTransport.${index}.ThoiGianTraHang`}
                                render={({ field }) => (
                                  <DatePicker
                                    disabled={
                                      accountType && accountType === "NV"
                                        ? false
                                        : true
                                    }
                                    className="form-control"
                                    showTimeSelect
                                    timeFormat="HH:mm"
                                    dateFormat="dd/MM/yyyy HH:mm"
                                    onChange={(date) => field.onChange(date)}
                                    selected={field.value}
                                  />
                                )}
                              />
                              {errors.listTransport?.[index]
                                ?.ThoiGianTraHang && (
                                <span className="text-danger">
                                  {
                                    errors.listTransport?.[index]
                                      ?.ThoiGianTraHang.message
                                  }
                                </span>
                              )}
                            </div>
                          </td>
                          <td>
                            <div className="form-group">
                              <input
                                readOnly={true}
                                type="text"
                                className="form-control"
                                id="KhoiLuong"
                                {...register(
                                  `listTransport.${index}.KhoiLuong`
                                )}
                              />
                            </div>
                          </td>
                          <td>
                            <div className="form-group">
                              <input
                                readOnly={true}
                                type="text"
                                className="form-control"
                                id="TheTich"
                                {...register(`listTransport.${index}.TheTich`)}
                              />
                            </div>
                          </td>
                          <td>
                            <div className="form-group">
                              <input
                                readOnly={true}
                                type="text"
                                className="form-control"
                                id="SoKien"
                                {...register(`listTransport.${index}.SoKien`)}
                              />
                            </div>
                          </td>

                          <td>
                            <div className="form-group">
                              {accountType && accountType === "NV" && (
                                <>
                                  {fields && fields.length > 0 && (
                                    <button
                                      type="button"
                                      className="btn btn-sm btn-default"
                                      onClick={() => {
                                        remove(index);
                                        handleOnChangeWeight(
                                          watch("PTVanChuyen")
                                        );
                                      }}
                                    >
                                      <i className="fas fa-minus"></i>
                                    </button>
                                  )}
                                </>
                              )}
                            </div>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
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
                  Xác Nhận
                </button>
              </div>
            </div>
          </form>
        )}
      </div>
    </>
  );
};

export default JoinTransports;
