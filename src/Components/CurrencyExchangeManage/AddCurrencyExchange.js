import { useState, useEffect } from "react";
import { getData, postData } from "../Common/FuncAxios";
import { useForm, Controller, useFieldArray } from "react-hook-form";
import DatePicker from "react-datepicker";
import moment from "moment";
import Select from "react-select";
import LoadingPage from "../Common/Loading/LoadingPage";

const AddCurrencyExchange = (props) => {
  const { getData, listCurrencyCode, listBanks, hideModal } = props;
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
      optionExchangeRate: [
        {
          bank: "",
          currencyCode: "",
          priceBuy: null,
          priceSell: null,
          priceTransfer: null,
          priceFix: null,
          createdTime: null,
          note: "",
        },
      ],
    },
  });

  const { fields, append, remove } = useFieldArray({
    control, // control props comes from useForm (optional: if you are using FormContext)
    name: "optionExchangeRate", // unique name for your Field Array
  });

  const Validate = {
    bank: { required: "Không được để trống" },
    currencyCode: { required: "Không được để trống" },
    priceBuy: {
      pattern: {
        value: /^[0-9.]*$/,
        message: "Chỉ được nhập ký tự là số",
      },
    },
    priceSell: {
      pattern: {
        value: /^[0-9.]*$/,
        message: "Chỉ được nhập ký tự là số",
      },
      required: "Không được để trống",
    },
    priceTransfer: {
      pattern: {
        value: /^[0-9.]*$/,
        message: "Chỉ được nhập ký tự là số",
      },
    },
    createdTime: { required: "Không được để trống" },
  };

  const [IsLoading, SetIsLoading] = useState(false);

  useEffect(() => {
    (async () => {})();
  }, []);

  const onSubmit = async (data) => {
    SetIsLoading(true);
    let arr = [];

    data.optionExchangeRate.forEach((val) => {
      arr.push({
        CurrencyCode: val.currencyCode,
        PriceBuy: !val.priceBuy ? null : val.priceBuy,
        PriceSell: val.priceSell,
        PriceTransfer: !val.priceTransfer ? null : val.priceTransfer,
        CreatedTime: val.createdTime,
        Bank: val.bank,
        PriceFix: !val.priceFix ? null : val.priceFix,
        Note: val.note,
      });
    });

    const create = await postData("CurrencyExchange/CreateExchangeRate", arr);
    if (create === 1) {
      getData();
      reset();
      hideModal();
    }
    SetIsLoading(false);
  };

  const handleResetClick = () => {};

  return (
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
              <th style={{ width: "10px" }}></th>
              <th>Ngân Hàng</th>
              <th>Mã Loại Tiền Tệ</th>
              <th>Tỉ Giá Bán</th>
              <th>Tỉ Giá Mua</th>
              <th>Tỉ Giá Chuyển khoản</th>
              <th>Ngày chuyển đổi</th>
              <th>Ghi Chú</th>
              <th style={{ width: "40px" }}></th>
            </tr>
          </thead>
          <tbody>
            {fields.map((value, index) => (
              <tr key={index}>
                <td>{index + 1}</td>
                <td>
                  <div className="form-group">
                    <select
                      className="form-control"
                      {...register(
                        `optionExchangeRate.${index}.bank`,
                        Validate.bank
                      )}
                    >
                      <option value="">Chọn Ngân Hàng</option>
                      {listBanks &&
                        listBanks.map((val) => {
                          return (
                            <option value={val.maNganHang} key={val.maNganHang}>
                              {val.maNganHang + " - " + val.tenNganHang}
                            </option>
                          );
                        })}
                    </select>
                    {errors.optionExchangeRate?.[index]?.bank && (
                      <span className="text-danger">
                        {errors.optionExchangeRate?.[index]?.bank.message}
                      </span>
                    )}
                  </div>
                </td>
                <td>
                  <div className="form-group">
                    <select
                      className="form-control"
                      {...register(
                        `optionExchangeRate.${index}.currencyCode`,
                        Validate.currencyCode
                      )}
                    >
                      <option value="">Chọn loại tiền tệ</option>
                      {listCurrencyCode &&
                        listCurrencyCode.map((val) => {
                          return (
                            <option
                              value={val.maLoaiTienTe}
                              key={val.maLoaiTienTe}
                            >
                              {val.maLoaiTienTe + " - " + val.tenLoaiTienTe}
                            </option>
                          );
                        })}
                    </select>
                    {errors.optionExchangeRate?.[index]?.currencyCode && (
                      <span className="text-danger">
                        {
                          errors.optionExchangeRate?.[index]?.currencyCode
                            .message
                        }
                      </span>
                    )}
                  </div>
                </td>
                <td>
                  <div className="form-group">
                    <input
                      type="number"
                      className="form-control"
                      id="priceSell"
                      {...register(
                        `optionExchangeRate.${index}.priceSell`,
                        Validate.priceSell
                      )}
                    />
                    {errors.optionExchangeRate?.[index]?.priceSell && (
                      <span className="text-danger">
                        {errors.optionExchangeRate?.[index]?.priceSell.message}
                      </span>
                    )}
                  </div>
                </td>
                <td>
                  <div className="form-group">
                    <input
                      type="number"
                      className="form-control"
                      id="priceBuy"
                      {...register(
                        `optionExchangeRate.${index}.priceBuy`,
                        Validate.priceBuy
                      )}
                    />
                    {errors.optionExchangeRate?.[index]?.priceBuy && (
                      <span className="text-danger">
                        {errors.optionExchangeRate?.[index]?.priceBuy.message}
                      </span>
                    )}
                  </div>
                </td>
                <td>
                  <div className="form-group">
                    <input
                      type="number"
                      className="form-control"
                      id="priceTransfer"
                      {...register(
                        `optionExchangeRate.${index}.priceTransfer`,
                        Validate.priceTransfer
                      )}
                    />
                    {errors.optionExchangeRate?.[index]?.priceTransfer && (
                      <span className="text-danger">
                        {
                          errors.optionExchangeRate?.[index]?.priceTransfer
                            .message
                        }
                      </span>
                    )}
                  </div>
                </td>
                <td>
                  <div className="form-group">
                    <div className="input-group ">
                      <Controller
                        control={control}
                        name={`optionExchangeRate.${index}.createdTime`}
                        render={({ field }) => (
                          <DatePicker
                            className="form-control"
                            dateFormat="dd/MM/yyyy"
                            onChange={(date) => field.onChange(date)}
                            selected={field.value}
                          />
                        )}
                        rules={Validate.createdTime}
                      />
                      {errors.optionExchangeRate?.[index]?.createdTime && (
                        <span className="text-danger">
                          {
                            errors.optionExchangeRate?.[index]?.createdTime
                              .message
                          }
                        </span>
                      )}
                    </div>
                  </div>
                </td>
                <td>
                  <div className="form-group">
                    <input
                      type="text"
                      className="form-control"
                      id="note"
                      {...register(
                        `optionExchangeRate.${index}.note`,
                        Validate.note
                      )}
                    />
                    {errors.optionExchangeRate?.[index]?.note && (
                      <span className="text-danger">
                        {errors.optionExchangeRate?.[index]?.note.message}
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
              <td colSpan={11}>
                <button
                  className="form-control form-control-sm"
                  type="button"
                  onClick={() => append(watch("optionExchangeRate")[0])}
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
  );
};

export default AddCurrencyExchange;
