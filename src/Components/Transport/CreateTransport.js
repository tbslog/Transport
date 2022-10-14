import { useState, useEffect } from "react";
import { getData, postData } from "../Common/FuncAxios";
import { useForm, Controller, useFieldArray } from "react-hook-form";
import DatePicker from "react-datepicker";
import Select from "react-select";
import { Tab, Tabs, TabList, TabPanel } from "react-tabs";

import moment from "moment/moment";

const CreateTransport = (props) => {
  const { listStatus } = props;
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

  const Validate = {};

  const [tabIndex, setTabIndex] = useState(0);
  const [listTransportType, setListTransportType] = useState([]);
  const [listStatusType, setListStatusType] = useState([]);
  const [listPoint, setListPoint] = useState([]);
  const [listPriceTable, setListPriceTable] = useState([]);
  const [listDVT, setListDVT] = useState([]);
  const [listPTVC, setListPTVC] = useState([]);
  const [listCustomer, setListCustomer] = useState([]);
  const [listNpp, setListNpp] = useState([]);
  const [listVehicleType, setlistVehicleType] = useState([]);
  const [listGoodsType, setListGoodsType] = useState([]);
  const [listDriver, setListDriver] = useState([]);
  const [listVehicle, setListVehicle] = useState([]);
  const [listRomooc, setListRomooc] = useState([]);
  const [listRoad, setListRoad] = useState([]);

  useEffect(() => {
    SetIsLoading(true);
    (async () => {
      let getListCustomer = await getData(
        `Customer/GetListCustomerOptionSelect`
      );
      if (getListCustomer && getListCustomer.length > 0) {
        let obj = [];

        getListCustomer
          .filter((x) => x.loaiKH == "KH")
          .map((val) => {
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

      setListDVT(getListDVT);
      setlistVehicleType(getListVehicleType);
      setListGoodsType(getListGoodsType);
      setListPTVC(getListTransportType);

      SetIsLoading(false);
    })();
  }, []);

  const handleOnChangeMaKH = async (value) => {
    SetIsLoading(true);

    setValue("KhachHang", value);
    let getListRoad = await getData(
      `Road/GetListRoadOptionSelect?MaKH=${value.value}`
    );
    if (getListRoad && getListRoad.length > 0) {
      let obj = [];
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

  const HandleOnChangeTabs = (tabIndex) => {
    setTabIndex(tabIndex, reset());
  };

  const onSubmit = async (data) => {
    SetIsLoading(true);
    console.log(data);
    SetIsLoading(false);
  };

  const handleResetClick = () => {
    reset();
  };
  return (
    <>
      <Tabs
        selectedIndex={tabIndex}
        onSelect={(index) => HandleOnChangeTabs(index)}
      >
        <TabList>
          <Tab>Tạo Vận Đơn Nhập</Tab>
          <Tab>Tạo Vận Đơn Xuất</Tab>
        </TabList>

        <TabPanel>
          {tabIndex === 0 && (
            <div className="card card-primary">
              <div className="card-header">
                <h3 className="card-title">Form Thêm Mới Vận Đơn Nhập</h3>
              </div>
              <div>{IsLoading === true && <div>Loading...</div>}</div>

              {IsLoading === false && (
                <form onSubmit={handleSubmit(onSubmit)}>
                  <div className="card-body">
                    <div className="row">
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
                    <div className="row">
                      <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="PTVanChuyen">
                            Phương tiện vận chuyển
                          </label>
                          <select
                            className="form-control"
                            {...register(`PTVanChuyen`, Validate.PTVanChuyen)}
                            value={watch("PTVanChuyen")}
                          >
                            <option value="">
                              Chọn phương Tiện Vận Chuyển
                            </option>
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
                          {errors.PTVanChuyen && (
                            <span className="text-danger">
                              {errors.PTVanChuyen.message}
                            </span>
                          )}
                        </div>
                      </div>
                      <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="LoaiHangHoa">Loại Hàng Hóa</label>
                          <select
                            className="form-control"
                            {...register(`LoaiHangHoa`, Validate.LoaiHangHoa)}
                            value={watch("LoaiHangHoa")}
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
                      </div>
                      <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="PTVC">Phương thức vận chuyển</label>
                          <select
                            className="form-control"
                            {...register("PTVC", Validate.PTVC)}
                            value={watch("PTVC")}
                          >
                            <option value="">
                              Chọn phương thức vận chuyển
                            </option>
                            {listPTVC &&
                              listPTVC.map((val) => {
                                return (
                                  <option value={val.maPtvc} key={val.maPtvc}>
                                    {val.tenPtvc}
                                  </option>
                                );
                              })}
                          </select>
                          {errors.PTVC && (
                            <span className="text-danger">
                              {errors.PTVC.message}
                            </span>
                          )}
                        </div>
                      </div>
                      <div className="col col-sm">
                        <div className="form-group">
                          <label htmlFor="DVT">Đơn vị tính</label>
                          <select
                            className="form-control"
                            {...register("DVT", Validate.DVT)}
                            value={watch("DVT")}
                          >
                            <option value="">Chọn Đơn Vị Tính</option>
                            {listDVT &&
                              listDVT.map((val) => {
                                return (
                                  <option value={val.maDvt} key={val.maDvt}>
                                    {val.tenDvt}
                                  </option>
                                );
                              })}
                          </select>
                          {errors.DVT && (
                            <span className="text-danger">
                              {errors.DVT.message}
                            </span>
                          )}
                        </div>
                      </div>
                    </div>
                    {watch("PTVanChuyen") &&
                      watch("PTVanChuyen").includes("CONT") && (
                        <>
                          <div className="row">
                            <div className="col col-sm">
                              <div className="form-group">
                                <label htmlFor="SoLuongCont">
                                  Số Lượng Container
                                </label>
                                <input
                                  autoComplete="false"
                                  type="text"
                                  className="form-control"
                                  id="SoLuongCont"
                                  {...register(
                                    `SoLuongCont`,
                                    Validate.SoLuongCont
                                  )}
                                />
                                {errors.SoLuongCont && (
                                  <span className="text-danger">
                                    {errors.SoLuongCont.message}
                                  </span>
                                )}
                              </div>
                            </div>
                            <div className="col col-sm">
                              <div className="form-group">
                                <label htmlFor="TongTrongLuong">
                                  Tổng Trọng Lượng
                                </label>
                                <input
                                  autoComplete="false"
                                  type="text"
                                  className="form-control"
                                  id="TongTrongLuong"
                                  {...register(
                                    `TongTrongLuong`,
                                    Validate.TongTrongLuong
                                  )}
                                />
                                {errors.TongTrongLuong && (
                                  <span className="text-danger">
                                    {errors.TongTrongLuong.message}
                                  </span>
                                )}
                              </div>
                              <br />
                            </div>
                          </div>
                          <div className="row">
                            <div className="col col-sm">
                              <div className="form-group">
                                <label htmlFor="TGLayRong">
                                  Thời Gian Lấy Rỗng
                                </label>
                                <div className="input-group ">
                                  <Controller
                                    control={control}
                                    name={`TGLayRong`}
                                    render={({ field }) => (
                                      <DatePicker
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
                                  {errors.TGLayRong && (
                                    <span className="text-danger">
                                      {errors.TGLayRong.message}
                                    </span>
                                  )}
                                </div>
                              </div>
                            </div>

                            <div className="col col-sm">
                              <div className="form-group">
                                <label htmlFor="TGXuatHang">
                                  Thời gian Hạ hàng
                                </label>
                                <div className="input-group ">
                                  <Controller
                                    control={control}
                                    name={`TGXuatHang`}
                                    render={({ field }) => (
                                      <DatePicker
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
                                  {errors.TGXuatHang && (
                                    <span className="text-danger">
                                      {errors.TGXuatHang.message}
                                    </span>
                                  )}
                                </div>
                              </div>
                            </div>
                          </div>
                        </>
                      )}

                    {watch("PTVanChuyen") &&
                      watch("PTVanChuyen").includes("TRUCK") && (
                        <>
                          <>
                            <div className="row">
                              <div className="col col-sm">
                                <div className="form-group">
                                  <label htmlFor="SoLuongCont">
                                    Số Lượng Thùng Hàng
                                  </label>
                                  <input
                                    autoComplete="false"
                                    type="text"
                                    className="form-control"
                                    id="SoLuongCont"
                                    {...register(
                                      `SoLuongCont`,
                                      Validate.SoLuongCont
                                    )}
                                  />
                                  {errors.SoLuongCont && (
                                    <span className="text-danger">
                                      {errors.SoLuongCont.message}
                                    </span>
                                  )}
                                </div>
                              </div>
                              <div className="col col-sm">
                                <div className="form-group">
                                  <label htmlFor="TongTrongLuong">
                                    Tổng Trọng Lượng
                                  </label>
                                  <input
                                    autoComplete="false"
                                    type="text"
                                    className="form-control"
                                    id="TongTrongLuong"
                                    {...register(
                                      `TongTrongLuong`,
                                      Validate.TongTrongLuong
                                    )}
                                  />
                                  {errors.TongTrongLuong && (
                                    <span className="text-danger">
                                      {errors.TongTrongLuong.message}
                                    </span>
                                  )}
                                </div>
                                <br />
                              </div>
                            </div>
                            <div className="row">
                              <div className="col col-sm">
                                <div className="form-group">
                                  <label htmlFor="TGLayHang">
                                    Thời Gian Lấy Hàng
                                  </label>
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
                                  <label htmlFor="TGXuatHang">
                                    Thời Gian Trả Hàng
                                  </label>
                                  <div className="input-group ">
                                    <Controller
                                      control={control}
                                      name={`TGXuatHang`}
                                      render={({ field }) => (
                                        <DatePicker
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
                                    {errors.TGXuatHang && (
                                      <span className="text-danger">
                                        {errors.TGXuatHang.message}
                                      </span>
                                    )}
                                  </div>
                                </div>
                              </div>
                            </div>
                          </>
                        </>
                      )}
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
          )}
        </TabPanel>
        <TabPanel>
          {tabIndex === 1 && (
            <div className="card card-primary">
              <div className="card-header">
                <h3 className="card-title">Form Thêm Mới Vận Đơn Xuất</h3>
              </div>
              <div>{IsLoading === true && <div>Loading...</div>}</div>

              {IsLoading === false && (
                <form onSubmit={handleSubmit(onSubmit)}>
                  <div className="card-body">
                    <div className="form-group">
                      <label htmlFor="TrangThai">Trạng thái</label>
                      <select
                        className="form-control"
                        {...register("TrangThai", Validate.TrangThai)}
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
          )}
        </TabPanel>
      </Tabs>
    </>
  );
};

export default CreateTransport;
