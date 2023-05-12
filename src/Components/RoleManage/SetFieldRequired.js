import { useState, useEffect } from "react";
import { getData, postData, getDataCustom } from "../Common/FuncAxios";
import { useForm, Controller } from "react-hook-form";
import CheckboxTree from "react-checkbox-tree";
import "react-checkbox-tree/lib/react-checkbox-tree.css";
import { ToastError } from "../Common/FuncToast";
import LoadingPage from "../Common/Loading/LoadingPage";
import Select from "react-select";

const SetFieldRequired = () => {
  const {
    register,
    reset,
    setValue,
    control,
    watch,
    validate,
    formState: { errors },
    handleSubmit,
  } = useForm({
    mode: "onChange",
  });

  const Validate = {
    MaKh: {
      required: "Không được để trống",
    },
  };

  const [IsLoading, SetIsLoading] = useState(true);
  const [checked, setChecked] = useState([]);
  const [expanded, setExpanded] = useState([]);
  const [nodes, setNodes] = useState([]);

  const [listCus, setListCus] = useState([]);
  const [listAccountCus, setListAccountCus] = useState([]);

  useEffect(() => {
    (async () => {
      (async () => {
        const getListCus = await getData(
          `Customer/GetListCustomerFilter?type=KH`
        );
        if (getListCus && getListCus.length > 0) {
          let arrKh = [];

          getListCus
            .filter((x) => x.loaiKH === "KH")
            .map((val) => {
              arrKh.push({
                label: val.maKh + " - " + val.tenKh,
                value: val.maKh,
              });
            });
          setListCus(arrKh);
        } else {
          setListCus([]);
        }

        SetIsLoading(false);
      })();
    })();
  }, []);

  const handleOnChangeCustomer = async (val) => {
    if (val && Object.keys(val).length > 0) {
      setListAccountCus([]);
      setValue("AccountCus", null);
      setValue("MaKH", val);
      const getListAcc = await getData(
        `AccountCustomer/GetListAccountSelectByCus?cusId=${val.value}`
      );

      var obj = [];
      obj.push({ label: "-- Để Trống --", value: null });
      if (getListAcc && getListAcc.length > 0) {
        getListAcc.map((val) => {
          obj.push({
            value: val.accountId,
            label: val.accountId + " - " + val.accountName,
          });
        });
        setListAccountCus(obj);
      } else {
        setListAccountCus(obj);
      }
    }
  };

  const handleOnChangeAccount = async (val) => {
    if (val) {
      SetIsLoading(true);
      let cusid = watch("MaKH");
      setValue("AccountCus", val);
      let getDataTree = await getData(
        `User/GetTreeFieldRequired?cusId=${cusid.value}&accId=${
          !val.value ? "" : val.value
        }`
      );

      setNodes(getDataTree.listTree);
      setChecked(getDataTree.isChecked);
      SetIsLoading(false);
    }
  };

  const onSubmit = async (data) => {
    if (!checked || checked.length < 1) {
      ToastError("Vui Lòng Chọn Quyền");
      return;
    }
    SetIsLoading(true);

    const creaetUser = await postData("User/SetRequiredField", {
      cusId: data.MaKH.value,
      accId: !data.AccountCus ? null : data.AccountCus.value,
      fields: checked,
    });

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
                  <label htmlFor="MaKH">Khách Hàng(*)</label>
                  <Controller
                    name="MaKH"
                    control={control}
                    render={({ field }) => (
                      <Select
                        {...field}
                        classNamePrefix={"form-control"}
                        options={listCus}
                        onChange={(field) => handleOnChangeCustomer(field)}
                        value={field.value}
                      />
                    )}
                    rules={Validate.MaKH}
                  />
                  {errors.MaKH && (
                    <span className="text-danger">{errors.MaKH.message}</span>
                  )}
                </div>
              </div>
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
                        options={listAccountCus}
                        onChange={(field) => handleOnChangeAccount(field)}
                        value={field.value}
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
            </div>

            <div className="row">
              <div className="form-group">
                <label htmlFor="TrangThai">Trường bắt buộc(*)</label>
                <CheckboxTree
                  iconsClass="fa5"
                  nodes={nodes}
                  checked={checked}
                  expanded={expanded}
                  showExpandAll={true}
                  showNodeIcon={true}
                  onCheck={(checked) => setChecked(checked)}
                  onExpand={(expanded) => setExpanded(expanded)}
                />
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
                Thêm mới
              </button>
            </div>
          </div>
        </form>
      )}
    </div>
  );
};

export default SetFieldRequired;
