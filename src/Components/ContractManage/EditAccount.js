import { useState, useEffect } from "react";
import { getData, postData } from "../Common/FuncAxios";
import { useForm, Controller, get } from "react-hook-form";
import LoadingPage from "../Common/Loading/LoadingPage";
import Select from "react-select";

const EditAccount = (props) => {
  const { selectIdClick, getListUser } = props;
  const [IsLoading, SetIsLoading] = useState(true);

  const {
    register,
    setValue,
    control,
    watch,
    formState: { errors },
    handleSubmit,
  } = useForm({
    mode: "onChange",
  });

  const Validate = {
    AccountName: {
      required: "Không được để trống",
    },
    ListCustomers: {
      required: "Không được để trống",
    },
  };

  const [listCustomer, setListCustomer] = useState([]);

  useEffect(() => {
    (async () => {
      let getListCustomer = await getData(
        `Customer/GetListCustomerFilter?type=KH`
      );
      if (getListCustomer && getListCustomer.length > 0) {
        let arrKh = [];
        getListCustomer.map((val) => {
          arrKh.push({
            label: val.tenKh,
            value: val.maKh,
          });
        });
        setListCustomer(arrKh);
      }
    })();
  }, []);

  useEffect(() => {
    if (
      listCustomer &&
      listCustomer.length > 0 &&
      selectIdClick &&
      Object.keys(selectIdClick).length > 0
    ) {
      (async () => {
        const getById = await getData(
          `AccountCustomer/GetAccountById?accountId=${selectIdClick.accountId}`
        );
        if (getById && Object.keys(getById).length > 0) {
          setValue("AccountName", getById.accountName);
          setValue(
            "ListCustomers",
            listCustomer.filter((x) => getById.listCustomer.includes(x.value))
          );
        }
      })();
    }
  }, [listCustomer, selectIdClick]);

  const onSubmit = async (data) => {
    SetIsLoading(true);
    let arrCus = [];
    data.ListCustomers.forEach((val) => {
      arrCus.push(val.value);
    });

    const post = await postData(
      `AccountCustomer/UpdateAccountCus?accountId=${selectIdClick.accountId}`,
      {
        AccountName: data.AccountName,
        ListCustomer: arrCus,
      }
    );

    if (post === 1) {
      getListUser();
    }
    SetIsLoading(false);
  };

  return (
    <>
      <form onSubmit={handleSubmit(onSubmit)}>
        <div className="card-body">
          <div className="row">
            <div className="col col-sm">
              <div className="form-group">
                <label htmlFor="AccountName">Tên Account</label>
                <input
                  type="text"
                  className="form-control"
                  id="AccountName"
                  readOnly={true}
                  {...register("AccountName", Validate.AccountName)}
                />
                {errors.AccountName && (
                  <span className="text-danger">
                    {errors.AccountName.message}
                  </span>
                )}
              </div>
            </div>
            <div className="col col-sm">
              <div className="form-group">
                <label htmlFor="ListCustomers">Chọn khách hàng</label>
                <Controller
                  name="ListCustomers"
                  control={control}
                  render={({ field }) => (
                    <Select
                      {...field}
                      className="basic-multi-select"
                      classNamePrefix={"form-control"}
                      isMulti
                      value={field.value}
                      options={listCustomer}
                      placeholder="Chọn Khách Hàng"
                    />
                  )}
                  rules={Validate.ListCustomers}
                />

                {errors.ListCustomers && (
                  <span className="text-danger">
                    {errors.ListCustomers.message}
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
    </>
  );
};

export default EditAccount;
