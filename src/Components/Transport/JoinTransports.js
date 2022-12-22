import { useState, useEffect } from "react";
import { getData, getDataCustom, postData } from "../Common/FuncAxios";
import { useForm, Controller, useFieldArray } from "react-hook-form";
import DatePicker from "react-datepicker";
import Select from "react-select";
import moment from "moment";

const JoinTransports = (props) => {
  const { items, clearItems, hideModal } = props;
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

  const { fields, append, remove } = useFieldArray({
    control, // control props comes from useForm (optional: if you are using FormContext)
    name: "listTransport", // unique name for your Field Array
  });

  const Validate = {
    PTVanChuyen: { required: "Không được để trống" },
    DiemLayTraRong: { required: "Không được để trống" },
    NhaCungCap: { required: "Không được để trống" },
    XeVanChuyen: { required: "Không được để trống" },
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

  useEffect(() => {
    (async () => {
      SetIsLoading(true);
      let getListVehicleType = await getData("Common/GetListVehicleType");
      setlistVehicleTypeSelect(getListVehicleType);
      setlistVehicleType(getListVehicleType);

      let getListGoodsType = await getData("Common/GetListGoodsType");
      setListGoodsType(getListGoodsType);

      const getListPoint = await getData("address/GetListAddressSelect");
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

      let getDataHandling = await getData("BillOfLading/LoadDataHandling");
      if (getDataHandling && Object.keys(getDataHandling).length > 0) {
        if (
          getDataHandling.listNhaPhanPhoi &&
          getDataHandling.listNhaPhanPhoi.length > 0
        ) {
          let arr = [];
          getDataHandling.listNhaPhanPhoi.map((val) => {
            arr.push({ label: val.tenNPP, value: val.maNPP });
          });
          setListSupplier(arr);
        }

        if (
          getDataHandling.listXeVanChuyen &&
          getDataHandling.listXeVanChuyen.length > 0
        ) {
          let arr = [];
          getDataHandling.listXeVanChuyen.map((val) => {
            arr.push({
              label: val.maSoXe + " - " + val.maLoaiPhuongTien,
              value: val.maSoXe,
            });
          });
          setListVehicle(arr);
          setListVehicleSelect(arr);
        }

        if (getDataHandling.listTaiXe && getDataHandling.listTaiXe.length > 0) {
          let arr = [];
          getDataHandling.listTaiXe.map((val) => {
            arr.push({
              label: val.maTaiXe + " - " + val.tenTaiXe,
              value: val.maTaiXe,
            });
          });
          setListDriver(arr);
        }

        if (
          getDataHandling.listRomooc &&
          getDataHandling.listRomooc.length > 0
        ) {
          let arr = [];
          getDataHandling.listRomooc.map((val) => {
            arr.push({
              label: val.maRomooc + " - " + val.tenLoaiRomooc,
              value: val.maRomooc,
            });
          });
          setListRomooc(arr);
        }
      }
      SetIsLoading(false);
    })();
  }, []);

  useEffect(() => {
    if (
      items &&
      items.length > 0 &&
      listVehicle &&
      listVehicleType &&
      listVehicle.length > 0 &&
      listVehicleType.length > 0
    ) {
      (async () => {
        resetForm();
        SetIsLoading(true);
        let arrTransport = [];
        items.map((val) => {
          arrTransport.push(val.maVanDon);
        });

        if (arrTransport && arrTransport.length > 0) {
          let dataTransports = await getDataCustom(
            `BillOfLading/LoadJoinTransports`,
            { TransportIds: arrTransport }
          );

          setValue("TransportType", dataTransports.loaiVanDon);
          setValue("MaPTVC", dataTransports.maPTVC);

          let data = dataTransports.loadTransports;
          setValue("listTransport", data);
          for (let i = 0; i <= data.length - 1; i++) {
            setValue(`listTransport.${i}.MaVanDon`, data[i].maVanDon);
            setValue(`listTransport.${i}.MaVanDonKH`, data[i].maVanDonKH);
            setValue(`listTransport.${i}.MaKH`, data[i].maKH);
            setValue(`listTransport.${i}.DiemDau`, data[i].diemDau);
            setValue(`listTransport.${i}.DiemCuoi`, data[i].diemCuoi);
            setValue(`listTransport.${i}.SoKhoi`, data[i].tongKhoiLuong);
            setValue(`listTransport.${i}.TheTich`, data[i].tongTheTich);
            setValue(`listTransport.${i}.SoKien`, data[i].tongSoKien);
          }
        }
      })();
    }
    SetIsLoading(false);
  }, [items, listVehicle, listVehicleType]);

  useEffect(() => {
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
    watch("listTransport").map((val) => {
      arrTransports.push({
        MaVanDon: val.maVanDon,
        LoaiHangHoa: val.LoaiHangHoa,
        MaDVT: "Chuyen",
      });
    });

    const dataJoin = {
      arrTransports: arrTransports,
      PTVanChuyen: data.PTVanChuyen,
      DiemLayTraRong: !data.DiemLayTraRong ? null : data.DiemLayTraRong.value,
      NhaCungCap: data.NhaCungCap.value,
      XeVanChuyen: data.XeVanChuyen.value,
      TaiXe: data.TaiXe.value,
      GhiChu: data.GhiChu,
      Romooc: !data.Romooc ? null : data.Romooc.value,
      CONTNO: !data.CONTNO ? null : data.CONTNO,
      SEALHQ: !data.SEALHQ ? null : data.SEALHQ,
      SEALNP: !data.SEALNP ? null : data.SEALNP,
      TGLayHang: !data.TGLayHang
        ? null
        : moment(new Date(data.TGLayHang).toISOString()).format(
            "yyyy-MM-DDTHH:mm:ss.SSS"
          ),
      TGTraHang: !data.TGTraHang
        ? null
        : moment(new Date(data.TGTraHang).toISOString()).format(
            "yyyy-MM-DDTHH:mm:ss.SSS"
          ),
      TGLayTraRong: !data.TGLayTraRong
        ? null
        : moment(new Date(data.TGLayTraRong).toISOString()).format(
            "yyyy-MM-DDTHH:mm:ss.SSS"
          ),
      TGHanLenh: !data.TGHanLenh
        ? null
        : moment(new Date(data.TGHanLenh).toISOString()).format(
            "yyyy-MM-DDTHH:mm:ss.SSS"
          ),
      TGHaCang: !data.TGHaCang
        ? null
        : moment(new Date(data.TGHaCang).toISOString()).format(
            "yyyy-MM-DDTHH:mm:ss.SSS"
          ),
    };

    console.log(dataJoin);
    // const create = await postData("BillOfLading/CreateHandlingLess", dataJoin);

    // if (create === 1) {
    //   clearItems();
    //   hideModal();
    // }

    SetIsLoading(false);
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
        <div className="card-header">
          <h3 className="card-title">Form Cập Nhật Điều Phối</h3>
        </div>
        <div>{IsLoading === true && <div>Loading...</div>}</div>

        {IsLoading === false && (
          <form onSubmit={handleSubmit(onSubmit)}>
            <div className="card-body">
              <div className="row">
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="TransportType">Loại Vận Đơn</label>
                    <input
                      readOnly={true}
                      autoComplete="false"
                      type="text"
                      className="form-control"
                      id="TransportType"
                      {...register(`TransportType`)}
                    />
                  </div>
                </div>
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="MaPTVC">Phương Thức Vận Chuyển</label>
                    <input
                      readOnly={true}
                      autoComplete="false"
                      type="text"
                      className="form-control"
                      id="MaPTVC"
                      {...register(`MaPTVC`)}
                    />
                  </div>
                </div>
              </div>
              <div className="row">
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="PTVanChuyen">
                      Phương tiện vận chuyển(*)
                    </label>
                    <select
                      className="form-control"
                      {...register(`PTVanChuyen`, Validate.PTVanChuyen)}
                      value={watch(`PTVanChuyen`)}
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
                {watch(`MaPTVC`) && watch(`MaPTVC`).includes("LCL") && (
                  <div className="col col-sm">
                    <div className="form-group">
                      {watch("TransportType") &&
                        watch("TransportType") === "xuat" && (
                          <label htmlFor="DiemLayTraRong">
                            Điểm Lấy Rỗng(*)
                          </label>
                        )}
                      {watch("TransportType") &&
                        watch("TransportType") === "nhap" && (
                          <label htmlFor="DiemLayTraRong">
                            Điểm Trả Rỗng(*)
                          </label>
                        )}
                      <Controller
                        name={`DiemLayTraRong`}
                        control={control}
                        render={({ field }) => (
                          <Select
                            {...field}
                            classNamePrefix={"form-control"}
                            value={field.value}
                            options={listPoint}
                          />
                        )}
                        rules={{
                          required: "không được để trống",
                        }}
                      />
                      {errors.DiemLayTraRong && (
                        <span className="text-danger">
                          {errors.DiemLayTraRong.message}
                        </span>
                      )}
                    </div>
                  </div>
                )}
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="NhaCungCap">Đơn Vị Vận Tải(*)</label>
                    <Controller
                      name={`NhaCungCap`}
                      control={control}
                      render={({ field }) => (
                        <Select
                          {...field}
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

                {/* <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="LoaiHangHoa">Loại Hàng Hóa(*)</label>
                    <select
                      className="form-control"
                      {...register(`LoaiHangHoa`, Validate.LoaiHangHoa)}
                      value={watch(`LoaiHangHoa`)}
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
                    {errors.LoaiHangHoa && (
                      <span className="text-danger">
                        {errors.LoaiHangHoa.message}
                      </span>
                    )}
                  </div>
                </div> */}
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
                      rules={{
                        required: "không được để trống",
                      }}
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
                      rules={{
                        required: "không được để trống",
                      }}
                    />
                    {errors.TaiXe && (
                      <span className="text-danger">
                        {errors.TaiXe.message}
                      </span>
                    )}
                  </div>
                </div>
                {watch(`MaPTVC`) && watch(`MaPTVC`).includes("LCL") && (
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
                {watch(`MaPTVC`) && watch(`MaPTVC`).includes("LCL") && (
                  <>
                    <div className="col col-sm">
                      <div className="form-group">
                        <label htmlFor="CONTNO">CONT NO(*)</label>
                        <input
                          autoComplete="false"
                          type="text"
                          className="form-control"
                          id="CONTNO"
                          {...register(`CONTNO`, Validate.CONTNO)}
                        />
                        {errors.CONTNO && (
                          <span className="text-danger">
                            {errors.CONTNO.message}
                          </span>
                        )}
                      </div>
                    </div>
                    <div className="col col-sm">
                      <div className="form-group">
                        <label htmlFor="SEALHQ">SEAL HQ</label>
                        <input
                          autoComplete="false"
                          type="text"
                          className="form-control"
                          id="SEALHQ"
                          {...register(`SEALHQ`, Validate.SEALHQ)}
                        />
                        {errors.SEALHQ && (
                          <span className="text-danger">
                            {errors.SEALHQ.message}
                          </span>
                        )}
                      </div>
                    </div>
                  </>
                )}

                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="SEALNP">SEAL NP</label>
                    <input
                      autoComplete="false"
                      type="text"
                      className="form-control"
                      id="SEALNP"
                      {...register(`SEALNP`, Validate.SEALNP)}
                    />
                    {errors.SEALNP && (
                      <span className="text-danger">
                        {errors.SEALNP.message}
                      </span>
                    )}
                  </div>
                </div>
              </div>

              <div className="row">
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="TGLayHang">Thời Gian Lấy Hàng</label>
                    <div className="input-group ">
                      <Controller
                        control={control}
                        name={`TGLayHang`}
                        render={({ field }) => (
                          <DatePicker
                            className="form-control"
                            showTimeSelect
                            timeFormat="HH:mm"
                            dateFormat="dd/MM/yyyy HH:mm"
                            onChange={(date) => field.onChange(date)}
                            selected={field.value}
                          />
                        )}
                      />
                      {errors.TGLayHang && (
                        <span className="text-danger">
                          {errors.TGLayHang.message}
                        </span>
                      )}
                    </div>
                  </div>
                </div>
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="TGTraHang">Thời Gian Trả Hàng</label>
                    <div className="input-group ">
                      <Controller
                        control={control}
                        name={`TGTraHang`}
                        render={({ field }) => (
                          <DatePicker
                            className="form-control"
                            showTimeSelect
                            timeFormat="HH:mm"
                            dateFormat="dd/MM/yyyy HH:mm"
                            onChange={(date) => field.onChange(date)}
                            selected={field.value}
                          />
                        )}
                      />
                      {errors.TGTraHang && (
                        <span className="text-danger">
                          {errors.TGTraHang.message}
                        </span>
                      )}
                    </div>
                  </div>
                </div>
              </div>
              {watch(`MaPTVC`) && watch(`MaPTVC`).includes("LCL") && (
                <>
                  <div className="row">
                    <div className="col col-sm">
                      <div className="form-group">
                        {watch("TransportType") &&
                          watch("TransportType") === "xuat" && (
                            <label htmlFor="TGLayTraRong">
                              Thời Gian Lấy Rỗng
                            </label>
                          )}
                        {watch("TransportType") &&
                          watch("TransportType") === "nhap" && (
                            <label htmlFor="TGLayTraRong">
                              Thời Gian Trả Rỗng
                            </label>
                          )}

                        <div className="input-group ">
                          <Controller
                            control={control}
                            name={`TGLayTraRong`}
                            render={({ field }) => (
                              <DatePicker
                                className="form-control"
                                showTimeSelect
                                timeFormat="HH:mm"
                                dateFormat="dd/MM/yyyy HH:mm"
                                onChange={(date) => field.onChange(date)}
                                selected={field.value}
                              />
                            )}
                          />
                          {errors.TGLayTraRong && (
                            <span className="text-danger">
                              {errors.TGLayTraRong.message}
                            </span>
                          )}
                        </div>
                      </div>
                    </div>
                    {watch("TransportType") &&
                    watch("TransportType") === "nhap" ? (
                      <>
                        <div className="col col-sm">
                          <div className="form-group">
                            <label htmlFor="TGHanLenh">
                              Thời Gian Hạn Lệnh(*)
                            </label>
                            <div className="input-group ">
                              <Controller
                                control={control}
                                name={`TGHanLenh`}
                                render={({ field }) => (
                                  <DatePicker
                                    className="form-control"
                                    showTimeSelect
                                    timeFormat="HH:mm"
                                    dateFormat="dd/MM/yyyy HH:mm"
                                    onChange={(date) => field.onChange(date)}
                                    selected={field.value}
                                  />
                                )}
                                rules={{
                                  required: "không được để trống",
                                }}
                              />
                              {errors.TGHanLenh && (
                                <span className="text-danger">
                                  {errors.TGHanLenh.message}
                                </span>
                              )}
                            </div>
                          </div>
                        </div>
                      </>
                    ) : (
                      <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="TGHaCang">Thời Gian Hạ Cảng(*)</label>
                          <div className="input-group ">
                            <Controller
                              control={control}
                              name={`TGHaCang`}
                              render={({ field }) => (
                                <DatePicker
                                  className="form-control"
                                  showTimeSelect
                                  timeFormat="HH:mm"
                                  dateFormat="dd/MM/yyyy HH:mm"
                                  onChange={(date) => field.onChange(date)}
                                  selected={field.value}
                                />
                              )}
                              rules={{
                                required: "không được để trống",
                              }}
                            />
                            {errors.TGHaCang && (
                              <span className="text-danger">
                                {errors.TGHaCang.message}
                              </span>
                            )}
                          </div>
                        </div>
                      </div>
                    )}
                  </div>
                </>
              )}
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
                <table
                  className="table table-sm table-bordered "
                  style={{
                    whiteSpace: "nowrap",
                  }}
                >
                  <thead>
                    <tr>
                      <th style={{ width: "40px" }}></th>
                      <th>
                        <div className="row">
                          <div className="col-sm-2">Mã Vận Đơn KH</div>
                          <div className="col-sm-3">Khách Hàng</div>
                          <div className="col-sm-2">Điểm Đầu</div>
                          <div className="col-sm-2">Điểm Cuối</div>
                          <div className="col-sm-1">Khối Lượng</div>
                          <div className="col-sm-1">Thể Tích</div>
                          <div className="col-sm-1">Số Kiện</div>
                        </div>
                      </th>
                    </tr>
                  </thead>
                  <tbody>
                    {fields.map((value, index) => (
                      <tr key={index}>
                        <td>{index + 1}</td>
                        <td>
                          <div className="row">
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
                            <div className="col-sm-2">
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
                            </div>
                            <div className="col-sm-3">
                              <div className="form-group">
                                <input
                                  readOnly={true}
                                  type="text"
                                  className="form-control"
                                  id="MaKH"
                                  {...register(`listTransport.${index}.MaKH`)}
                                />
                              </div>
                            </div>
                            <div className="col-sm-2">
                              <div className="form-group">
                                <input
                                  readOnly={true}
                                  type="text"
                                  className="form-control"
                                  id="DiemDau"
                                  {...register(
                                    `listTransport.${index}.DiemDau`
                                  )}
                                />
                              </div>
                            </div>
                            <div className="col-sm-2">
                              <div className="form-group">
                                <input
                                  readOnly={true}
                                  type="text"
                                  className="form-control"
                                  id="DiemCuoi"
                                  {...register(
                                    `listTransport.${index}.DiemCuoi`
                                  )}
                                />
                              </div>
                            </div>
                            <div className="col-sm-1">
                              <div className="form-group">
                                <input
                                  readOnly={true}
                                  type="text"
                                  className="form-control"
                                  id="TongKhoiLuong"
                                  {...register(
                                    `listTransport.${index}.TongKhoiLuong`
                                  )}
                                />
                              </div>
                            </div>
                            <div className="col-sm-1">
                              <div className="form-group">
                                <input
                                  readOnly={true}
                                  type="text"
                                  className="form-control"
                                  id="TongTheTich"
                                  {...register(
                                    `listTransport.${index}.TongTheTich`
                                  )}
                                />
                              </div>
                            </div>
                            <div className="col-sm-1">
                              <div className="form-group">
                                <input
                                  readOnly={true}
                                  type="text"
                                  className="form-control"
                                  id="TongSoKien"
                                  {...register(
                                    `listTransport.${index}.TongSoKien`
                                  )}
                                />
                              </div>
                            </div>
                          </div>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
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
