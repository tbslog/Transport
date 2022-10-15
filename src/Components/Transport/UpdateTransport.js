import { useState, useEffect } from "react";
import { getData, postData } from "../Common/FuncAxios";
import { useForm, Controller, useFieldArray } from "react-hook-form";
import DatePicker from "react-datepicker";
import Select from "react-select";
import moment from "moment";
import { Tab, Tabs, TabList, TabPanel } from "react-tabs";

const UpdateTransport = (props) => {
  const { getListTransport, selectIdClick, hideModal } = props;

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

  const Validate = {
    MaCungDuong: {
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
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt",
      },
    },
    LoaiHangHoa: {
      required: "Không được để trống",
    },
    PTVC: {
      required: "Không được để trống",
    },
    DVT: {
      required: "Không được để trống",
    },
    TongThungHang: {
      required: "Không được để trống",
      pattern: {
        value: /^[0-9]*$/,
        message: "Chỉ được nhập ký tự là số",
      },
    },
    TongKhoiLuong: {
      required: "Không được để trống",
      pattern: {
        pattern: {
          value: /^[0-9]*$/,
          message: "Chỉ được nhập ký tự là số",
        },
      },
    },
  };

  const [tabIndex, setTabIndex] = useState(0);
  const [IsLoading, SetIsLoading] = useState(false);
  const [listPoint, setListPoint] = useState([]);
  const [listRoad, setListRoad] = useState([]);
  const [contractId, setContractId] = useState("");

  const HandleOnChangeTabs = (tabIndex) => {
    setTabIndex(tabIndex, reset());
  };

  useEffect(() => {
    SetIsLoading(true);
    (async () => {
      let getListRoad = await getData(`Road/GetListRoadOptionSelect`);
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
    })();
  }, []);

  useEffect(() => {
    if (props && selectIdClick && Object.keys(selectIdClick).length > 0) {
      setValue(
        "MaCungDuong",
        {
          ...listRoad.filter((x) => x.value === selectIdClick.maCungDuong),
        }[0]
      );

      console.log(selectIdClick);
      setContractId(selectIdClick.maVanDon);
      setValue("MaVanDon", selectIdClick.maVanDon);
      setValue("TongThungHang", selectIdClick.tongThungHang);
      setValue("TongKhoiLuong", selectIdClick.tongKhoiLuong);
      setValue("TGLayHang", new Date(selectIdClick.thoiGianLayHang));
      setValue("TGTraHang", new Date(selectIdClick.thoiGianTraHang));

      if (selectIdClick.loaiVanDon == "nhap") {
      }
    }
  }, [props, selectIdClick, listRoad]);

  const onSubmit = async (data) => {
    SetIsLoading(true);

    const Update = await postData(
      `BillOfLading/UpdateTransport?transportId=${contractId}`,
      {
        maCungDuong: data.MaCungDuong.value,
        tongThungHang: data.TongThungHang,
        tongKhoiLuong: data.TongKhoiLuong,
        thoiGianLayHang: moment(new Date(data.TGLayHang).toISOString()).format(
          "yyyy-MM-DDTHH:mm:ss.SSS"
        ),
        thoiGianTraHang: moment(new Date(data.TGTraHang).toISOString()).format(
          "yyyy-MM-DDTHH:mm:ss.SSS"
        ),
      }
    );

    if (Update === 1) {
      getListTransport(1);
      hideModal();
    }

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
                          <label htmlFor="MaVanDon">Mã Vận Đơn</label>
                          <input
                            readOnly
                            autoComplete="false"
                            type="text"
                            className="form-control"
                            id="MaVanDon"
                            {...register(`MaVanDon`)}
                          />
                          {errors.MaVanDon && (
                            <span className="text-danger">
                              {errors.MaVanDon.message}
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
                    <>
                      <div className="row">
                        <div className="col col-sm">
                          <div className="form-group">
                            <label htmlFor="TongThungHang">
                              Tổng Thùng Hàng
                            </label>
                            <input
                              autoComplete="false"
                              type="text"
                              className="form-control"
                              id="TongThungHang"
                              {...register(
                                `TongThungHang`,
                                Validate.TongThungHang
                              )}
                            />
                            {errors.TongThungHang && (
                              <span className="text-danger">
                                {errors.TongThungHang.message}
                              </span>
                            )}
                          </div>
                        </div>
                        <div className="col col-sm">
                          <div className="form-group">
                            <label htmlFor="TongKhoiLuong">
                              Tổng Trọng Lượng
                            </label>
                            <input
                              autoComplete="false"
                              type="text"
                              className="form-control"
                              id="TongKhoiLuong"
                              {...register(
                                `TongKhoiLuong`,
                                Validate.TongKhoiLuong
                              )}
                            />
                            {errors.TongKhoiLuong && (
                              <span className="text-danger">
                                {errors.TongKhoiLuong.message}
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
                                    onChange={(date) => field.onChange(date)}
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
                            <label htmlFor="TGTraHang">
                              Thời Gian Trả Hàng
                            </label>
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
                                rules={{
                                  required: "không được để trống",
                                }}
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
                    </>
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
                        Cập nhật
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
                  <div className="card-body"></div>
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
          )}
        </TabPanel>
      </Tabs>
    </>
  );
};

export default UpdateTransport;
