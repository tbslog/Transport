import { useState, useEffect } from "react";
import { getData, postData } from "../Common/FuncAxios";
import { useForm, Controller, useFieldArray } from "react-hook-form";
import moment from "moment";
import Select from "react-select";
import LoadingPage from "../Common/Loading/LoadingPage";

const CreateSubfeeLolo = (props) => {
  const { refeshData } = props;
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
    defaultValues: {
      optionSfLolo: [
        {
          MaDiaDiem: null,
          MaKh: null,
          HangTau: null,
          LoaiCont: null,
          DonGia: null,
          LoaiPhuPhi: 1,
        },
      ],
    },
  });

  const { fields, append, remove } = useFieldArray({
    control, // control props comes from useForm (optional: if you are using FormContext)
    name: "optionSfLolo", // unique name for your Field Array
  });

  const Validate = {
    MaKh: {
      maxLength: {
        value: 8,
        message: "Không được vượt quá 8 ký tự",
      },
      minLength: {
        value: 8,
        message: "Không được ít hơn 8 ký tự",
      },
    },
    DonGia: {
      pattern: {
        value: /^[0-9]*$/,
        message: "Chỉ được nhập ký tự là số",
      },
      required: "Không được để trống",
    },
    LoaiPhuPhi: {
      required: "Không được để trống",
    },
    LoaiCont: {
      required: "Không được để trống",
    },
    MaDiaDiem: {
      required: "không được để trống",
      validate: (value) => {
        if (!value.value) {
          return "không được để trống";
        }
      },
    },
  };

  const [IsLoading, SetIsLoading] = useState(false);
  const [listCustomer, setListCustomer] = useState([]);
  const [listVehicleType, setListVehicleType] = useState([]);
  const [listPlace, setListPlace] = useState([]);
  const [listShip, setListShip] = useState([]);

  useEffect(() => {
    SetIsLoading(true);
    (async () => {
      let getListVehicleType = await getData("Common/GetListVehicleType");
      const getListPlace = await getData(
        "Address/GetListAddressSelect?pointType=&type="
      );

      let arrPlace = [];
      getListPlace.forEach((val) => {
        arrPlace.push({
          label: val.tenDiaDiem + " - " + val.loaiDiaDiem,
          value: val.maDiaDiem,
        });
      });
      setListPlace(arrPlace);

      let getListCustomer = await getData(
        `Customer/GetListCustomerOptionSelect`
      );
      if (getListCustomer && getListCustomer.length > 0) {
        getListCustomer = getListCustomer.filter((x) => x.loaiKH === "KH");
        let obj = [];
        obj.push({ value: null, label: "Để Trống" });
        getListCustomer.map((val) => {
          obj.push({
            value: val.maKh,
            label: val.maKh + " - " + val.tenKh,
          });
        });
        setListCustomer(obj);
      }

      let getListShip = await getData("Common/GetListShipping");
      let objShip = [];
      objShip.push({ value: null, label: "Để Trống" });
      getListShip.forEach((val) => {
        objShip.push({ value: val.shippingCode, label: val.shippingLineName });
      });

      setListShip(objShip);
      setListVehicleType(getListVehicleType);
      SetIsLoading(false);
    })();
  }, []);

  const handleResetClick = () => {
    reset();
  };

  const onSubmit = async (data) => {
    SetIsLoading(true);
    let arr = [];

    data.optionSfLolo.forEach((val) => {
      arr.push({
        MaDiaDiem: val.MaDiaDiem.value,
        MaKh: !val.MaKh ? null : val.MaKh.value,
        HangTau: !val.HangTau ? null : val.HangTau.value,
        LoaiCont: val.LoaiCont,
        LoaiPhuPhi: parseInt(val.LoaiPhuPhi),
        DonGia: parseFloat(val.DonGia),
      });
    });

    const createPriceTable = await postData("SubfeeLolo/CreateSubfeeLolo", arr);
    if (createPriceTable === 1) {
      reset();
      await refeshData();
    }
    SetIsLoading(false);
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
              <br />
              <table
                className="table table-sm table-bordered"
                style={{
                  whiteSpace: "nowrap",
                }}
              >
                <thead>
                  <tr>
                    <th></th>
                    <th>Địa Điểm(*)</th>
                    <th>Hãng Tàu</th>
                    <th>Khách hàng</th>
                    <th>Loại Phụ Phí(*)</th>
                    <th>Loại phương tiện(*)</th>
                    <th>Đơn Giá (*)</th>
                    <th style={{ width: "40px" }}></th>
                  </tr>
                </thead>
                <tbody>
                  {fields.map((value, index) => (
                    <tr key={index}>
                      <td>{index + 1}</td>
                      <td>
                        <div className="form-group">
                          <Controller
                            name={`optionSfLolo.${index}.MaDiaDiem`}
                            control={control}
                            render={({ field }) => (
                              <Select
                                {...field}
                                classNamePrefix={"form-control"}
                                value={field.value}
                                options={listPlace}
                                placeholder={"Điểm Nâng/Hạ"}
                              />
                            )}
                          />
                          {errors.optionSfLolo?.[index]?.MaDiaDiem && (
                            <span className="text-danger">
                              {errors.optionSfLolo?.[index]?.MaDiaDiem.message}
                            </span>
                          )}
                        </div>
                      </td>
                      <td>
                        <div className="form-group">
                          <Controller
                            name={`optionSfLolo.${index}.HangTau`}
                            control={control}
                            render={({ field }) => (
                              <Select
                                {...field}
                                classNamePrefix={"form-control"}
                                value={field.value}
                                options={listShip}
                                placeholder={"Hãng Tàu"}
                              />
                            )}
                          />
                          {errors.optionSfLolo?.[index]?.HangTau && (
                            <span className="text-danger">
                              {errors.optionSfLolo?.[index]?.HangTau.message}
                            </span>
                          )}
                        </div>
                      </td>
                      <td>
                        <div className="form-group">
                          <Controller
                            {...register(
                              `optionSfLolo.${index}.MaKh`,
                              Validate.MaKh
                            )}
                            control={control}
                            render={({ field }) => (
                              <Select
                                {...field}
                                classNamePrefix={"form-control"}
                                value={field.value}
                                options={listCustomer}
                                placeholder={"Chọn Khách Hàng"}
                              />
                            )}
                            rules={Validate.MaKh}
                          />
                          {errors.optionSfLolo?.[index]?.MaKh && (
                            <span className="text-danger">
                              {errors.optionSfLolo?.[index]?.MaKh.message}
                            </span>
                          )}
                        </div>
                      </td>
                      <td>
                        <div className="form-group">
                          <select
                            className="form-control"
                            {...register(
                              `optionSfLolo.${index}.LoaiPhuPhi`,
                              Validate.LoaiPhuPhi
                            )}
                          >
                            <option value={1}>Phí Nâng</option>
                            <option value={2}>Phí Hạ</option>
                          </select>
                          {errors.optionSfLolo?.[index]?.LoaiPhuPhi && (
                            <span className="text-danger">
                              {errors.optionSfLolo?.[index]?.LoaiPhuPhi.message}
                            </span>
                          )}
                        </div>
                      </td>
                      <td>
                        <div className="form-group">
                          <select
                            className="form-control"
                            {...register(
                              `optionSfLolo.${index}.LoaiCont`,
                              Validate.LoaiCont
                            )}
                          >
                            <option value="">Chọn loại phương tiện</option>
                            {listVehicleType
                              .filter((x) =>
                                x.maLoaiPhuongTien.includes("CONT")
                              )
                              .map((val) => {
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
                          {errors.optionSfLolo?.[index]?.LoaiCont && (
                            <span className="text-danger">
                              {errors.optionSfLolo?.[index]?.LoaiCont.message}
                            </span>
                          )}
                        </div>
                      </td>
                      <td>
                        <div className="form-group">
                          <input
                            type="text"
                            className="form-control"
                            id="DonGia"
                            {...register(
                              `optionSfLolo.${index}.DonGia`,
                              Validate.DonGia
                            )}
                          />
                          {errors.optionSfLolo?.[index]?.DonGia && (
                            <span className="text-danger">
                              {errors.optionSfLolo?.[index]?.DonGia.message}
                            </span>
                          )}
                        </div>
                      </td>

                      <td>
                        <div className="form-group">
                          {index >= 1 && (
                            <>
                              <button
                                type="button"
                                className="form-control form-control-sm"
                                onClick={() => remove(index)}
                              >
                                <i className="fas fa-minus"></i>
                              </button>
                            </>
                          )}
                        </div>
                      </td>
                    </tr>
                  ))}
                  <tr>
                    <td colSpan={9}>
                      <button
                        className="form-control form-control-sm"
                        type="button"
                        onClick={() => append(watch("optionSfLolo")[0])}
                      >
                        <i className="fas fa-plus"></i>
                      </button>
                    </td>
                  </tr>
                </tbody>
              </table>
              <br />
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

export default CreateSubfeeLolo;
