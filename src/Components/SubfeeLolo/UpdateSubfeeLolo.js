import { useState, useEffect } from "react";
import { getData, postData } from "../Common/FuncAxios";
import { useForm, Controller } from "react-hook-form";
import Select from "react-select";
import LoadingPage from "../Common/Loading/LoadingPage";

const UpdateSubfeeLolo = (props) => {
  const { selectIdClick, refeshData, hideModal } = props;
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

  useEffect(() => {
    if (
      listCustomer.length > 0 &&
      listVehicleType.length > 0 &&
      listPlace.length > 0 &&
      listShip.length > 0
    ) {
      (async () => {
        let getDataById = await getData(
          `SubfeeLolo/GetSubfeeLoloById?id=${selectIdClick.id}`
        );

        if (getDataById && Object.keys(getDataById).length > 0) {
          setValue(
            "MaDiaDiem",
            listPlace.find((x) => x.value === getDataById.maDiaDiem)
          );
          setValue(
            "HangTau",
            listShip.find((x) => x.value === getDataById.hangTau)
          );
          setValue(
            "MaKh",
            listCustomer.find((x) => x.value === getDataById.maKh)
          );
          setValue("LoaiPhuPhi", getDataById.loaiPhuPhi);
          setValue("LoaiCont", getDataById.loaiCont);
          setValue("DonGia", getDataById.donGia);
        }
      })();
    }
  }, [selectIdClick, listCustomer, listVehicleType, listPlace, listShip]);

  const handleResetClick = () => {
    reset();
  };

  const onSubmit = async (data) => {
    SetIsLoading(true);

    const createPriceTable = await postData(
      `SubfeeLolo/UpdateSubfeeLolo?id=${selectIdClick.id}`,
      {
        MaDiaDiem: data.MaDiaDiem.value,
        MaKh: !data.MaKh ? null : data.MaKh.value,
        HangTau: !data.HangTau ? null : data.HangTau.value,
        LoaiCont: data.LoaiCont,
        LoaiPhuPhi: parseInt(data.LoaiPhuPhi),
        DonGia: parseFloat(data.DonGia),
      }
    );
    if (createPriceTable === 1) {
      reset();
      await refeshData();
      hideModal();
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
                    <th>Địa Điểm(*)</th>
                    <th>Hãng Tàu</th>
                    <th>Khách hàng</th>
                    <th>Loại Phụ Phí(*)</th>
                    <th>Loại phương tiện(*)</th>
                    <th>Đơn Giá (*)</th>
                  </tr>
                </thead>
                <tbody>
                  <tr>
                    <td>
                      <div className="form-group">
                        <Controller
                          name={`MaDiaDiem`}
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
                        {errors.MaDiaDiem && (
                          <span className="text-danger">
                            {errors.MaDiaDiem.message}
                          </span>
                        )}
                      </div>
                    </td>
                    <td>
                      <div className="form-group">
                        <Controller
                          name={`HangTau`}
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
                        {errors.HangTau && (
                          <span className="text-danger">
                            {errors.HangTau.message}
                          </span>
                        )}
                      </div>
                    </td>
                    <td>
                      <div className="form-group">
                        <Controller
                          {...register(`MaKh`, Validate.MaKh)}
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
                        {errors.MaKh && (
                          <span className="text-danger">
                            {errors.MaKh.message}
                          </span>
                        )}
                      </div>
                    </td>
                    <td>
                      <div className="form-group">
                        <select
                          className="form-control"
                          {...register(`LoaiPhuPhi`, Validate.LoaiPhuPhi)}
                        >
                          <option value={1}>Phí Nâng</option>
                          <option value={2}>Phí Hạ</option>
                        </select>
                        {errors.LoaiPhuPhi && (
                          <span className="text-danger">
                            {errors.LoaiPhuPhi.message}
                          </span>
                        )}
                      </div>
                    </td>
                    <td>
                      <div className="form-group">
                        <select
                          className="form-control"
                          {...register(`LoaiCont`, Validate.LoaiCont)}
                        >
                          <option value="">Chọn loại phương tiện</option>
                          {listVehicleType
                            .filter((x) => x.maLoaiPhuongTien.includes("CONT"))
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
                        {errors.LoaiCont && (
                          <span className="text-danger">
                            {errors.LoaiCont.message}
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
                          {...register(`DonGia`, Validate.DonGia)}
                        />
                        {errors.DonGia && (
                          <span className="text-danger">
                            {errors.DonGia.message}
                          </span>
                        )}
                      </div>
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
                  Cập Nhật
                </button>
              </div>
            </div>
          </form>
        )}
      </div>
    </>
  );
};

export default UpdateSubfeeLolo;
