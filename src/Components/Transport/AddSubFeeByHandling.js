import { useState, useEffect, useRef, useMemo } from "react";
import { getData, postData } from "../Common/FuncAxios";
import { useForm, Controller, useFieldArray } from "react-hook-form";
import { Tab, Tabs, TabList, TabPanel } from "react-tabs";
import Select from "react-select";
import DataTable from "react-data-table-component";
import moment from "moment";
import { Modal } from "bootstrap";
import LoadingPage from "../Common/Loading/LoadingPage";

const AddSubFeeByHandling = (props) => {
  const { dataClick } = props;
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
    Payfor: {
      required: "Không được để trống",
    },
    DonGia: {
      required: "Không được để trống",
    },
  };

  const { fields, append, remove } = useFieldArray({
    control, // control props comes from useForm (optional: if you are using FormContext)
    name: "optionSubFee", // unique name for your Field Array
  });

  const [data, setData] = useState([]);
  const [ShowModal, SetShowModal] = useState("");
  const [modal, setModal] = useState(null);
  const parseExceptionModal = useRef();
  const [selectIdClick, setSelectIdClick] = useState({});

  const [IsLoading, SetIsLoading] = useState(true);
  const [tabIndex, setTabIndex] = useState(0);
  const [listSubFeeSelect, setListSubFeeSelect] = useState([]);
  const [listsfPayfor, setListsfPayfor] = useState([]);
  const [listPlace, setListPlace] = useState([]);

  useEffect(() => {
    (async () => {
      let getListSubFee = await getData(`SubFeePrice/GetListSubFeeSelect`);

      if (getListSubFee && getListSubFee.length > 0) {
        let obj = [];
        getListSubFee.map((val) => {
          obj.push({
            value: val.subFeeId,
            label: val.subFeeId + " - " + val.subFeeName,
          });
        });
        setListSubFeeSelect(obj);

        setValue("optionSubFee", [
          {
            MaPhuPhi: {},
            MaDiaDiem: {},
            DonGia: null,
            GhiChu: null,
          },
        ]);
      }

      let getlistSfPayfor = await getData(`Common/GetListsftPayfor`);
      setListsfPayfor(getlistSfPayfor);

      const getListPlace = await getData(
        "Address/GetListAddressSelect?pointType=&type="
      );

      let arrPlace = [];
      arrPlace.push({ value: null, label: "Bỏ Trống" });
      getListPlace.forEach((val) => {
        arrPlace.push({
          label: val.tenDiaDiem + " - " + val.loaiDiaDiem,
          value: val.maDiaDiem,
        });
      });
      setListPlace(arrPlace);

      SetIsLoading(false);
    })();
  }, []);

  useEffect(() => {
    if (props && dataClick && Object.keys(dataClick).length > 0) {
      fetchData(dataClick.maDieuPhoi);
    }
  }, [props, dataClick]);

  const columns = useMemo(() => [
    {
      selector: (row) => row.id,
      omit: true,
    },
    {
      name: "Mã Vận Đơn",
      selector: (row) => row.maVanDon,
      sortable: true,
    },
    {
      name: "Loại Phụ Phí",
      selector: (row) => <div className="text-wrap">{row.subFee}</div>,
      sortable: true,
    },
    {
      name: "Phân Loại",
      selector: (row) => row.type,
      sortable: true,
    },
    {
      name: "Đơn Giá",
      selector: (row) =>
        row.price.toLocaleString("vi-VI", {
          style: "currency",
          currency: "VND",
        }),
      sortable: true,
    },
    {
      name: "Loại Tiền Tệ",
      selector: (row) => row.priceType,
      sortable: true,
    },
    {
      name: "Đơn Vị Chi Trả",
      selector: (row) => row.payfor,
      sortable: true,
    },
    {
      name: "Mã Số Xe",
      selector: (row) => row.maSoXe,
      sortable: true,
    },
    {
      name: "Tài Xế",
      selector: (row) => row.taiXe,
      sortable: true,
    },
    {
      name: "Trạng Thái",
      selector: (row) => row.trangThai,
      sortable: true,
    },
    {
      name: "Thời Gian Tạo",
      selector: (row) => moment(row.createdDate).format("DD/MM/YYYY HH:mm:ss"),
      sortable: true,
    },
    {
      name: "Thời Gian Duyệt",
      selector: (row) => moment(row.approveDate).format("DD/MM/YYYY HH:mm:ss"),
      sortable: true,
    },
  ]);

  const showModalForm = () => {
    const modal = new Modal(parseExceptionModal.current, {
      keyboard: false,
      backdrop: "static",
    });
    setModal(modal);
    modal.show();
  };

  const handleEditButtonClick = (value) => {
    setSelectIdClick(value);

    showModalForm();
  };

  const fetchData = async (maDieuPhoi) => {
    SetIsLoading(true);
    const data = await getData(
      `SFeeByTcommand/GetListSubFeeIncurredByHandling?id=${maDieuPhoi}`
    );
    setData(data);
    SetIsLoading(false);
  };

  const hideModal = () => {
    modal.hide();
  };

  const onSubmit = async (data, e) => {
    SetIsLoading(true);

    let arr = [];
    data.optionSubFee.map((val) => {
      arr.push({
        IdTcommand: dataClick.maDieuPhoi,
        SfId: val.MaPhuPhi.value,
        PlaceId: !val.MaDiaDiem ? null : val.MaDiaDiem.value,
        sftPayfor: parseInt(val.Payfor),
        FinalPrice: val.DonGia,
        Note: val.GhiChu,
      });
    });

    if (arr && arr.length > 0) {
      const createPriceTable = await postData(
        "SFeeByTcommand/CreateSFeeByTCommand",
        arr
      );

      if (createPriceTable === 1) {
        setValue("optionSubFee", [
          {
            MaPhuPhi: {},
            MaDiaDiem: {},
            Payfor: null,
            DonGia: null,
            GhiChu: null,
          },
        ]);
      }
    }

    SetIsLoading(false);
  };

  const handleResetClick = () => {
    reset();
    hideModal();
  };

  const HandleOnChangeTabs = (tabIndex) => {
    setTabIndex(tabIndex);
  };

  return (
    <>
      <Tabs
        selectedIndex={tabIndex}
        onSelect={(index) => HandleOnChangeTabs(index)}
      >
        <TabList>
          <Tab>Thêm phụ phí phát sinh</Tab>
          <Tab>Danh sách phụ phí phát sinh (Đã Duyệt)</Tab>
        </TabList>

        <TabPanel>
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
                  <table
                    className="table table-sm table-bordered"
                    style={{
                      whiteSpace: "nowrap",
                    }}
                  >
                    <thead>
                      <tr>
                        <th style={{ width: "40px" }}></th>
                        <th></th>
                        <th style={{ width: "40px" }}>
                          <button
                            className="form-control form-control-sm"
                            type="button"
                            onClick={() =>
                              append({
                                MaPhuPhi: null,
                                DonGia: null,
                                MaDiaDiem: null,
                                GhiChu: null,
                              })
                            }
                          >
                            <i className="fas fa-plus"></i>
                          </button>
                        </th>
                      </tr>
                    </thead>
                    <tbody>
                      {fields.map((value, index) => (
                        <tr key={index}>
                          <td>{index + 1}</td>
                          <td>
                            <div className="row">
                              <div className="col-sm-3">
                                <div className="form-group">
                                  <label>Loại Phụ Phí</label>
                                  <Controller
                                    name={`optionSubFee.${index}.MaPhuPhi`}
                                    control={control}
                                    render={({ field }) => (
                                      <Select
                                        {...field}
                                        classNamePrefix={"form-control"}
                                        value={field.value}
                                        options={listSubFeeSelect}
                                      />
                                    )}
                                    rules={{ required: "không được để trống" }}
                                  />
                                  {errors.optionSubFee?.[index]?.MaPhuPhi && (
                                    <span className="text-danger">
                                      {
                                        errors.optionSubFee?.[index]?.MaPhuPhi
                                          .message
                                      }
                                    </span>
                                  )}
                                </div>
                              </div>
                              <div className="col-sm-2">
                                <div className="form-group">
                                  <label>Địa Điểm</label>
                                  <Controller
                                    name={`optionSubFee.${index}.MaDiaDiem`}
                                    control={control}
                                    render={({ field }) => (
                                      <Select
                                        {...field}
                                        classNamePrefix={"form-control"}
                                        value={field.value}
                                        options={listPlace}
                                      />
                                    )}
                                    rules={{ required: "không được để trống" }}
                                  />
                                  {errors.optionSubFee?.[index]?.MaDiaDiem && (
                                    <span className="text-danger">
                                      {
                                        errors.optionSubFee?.[index]?.MaDiaDiem
                                          .message
                                      }
                                    </span>
                                  )}
                                </div>
                              </div>
                              <div className="col-sm-2">
                                <div className="form-group">
                                  <label>Đơn Vị Chi Trả</label>
                                  <select
                                    className="form-control"
                                    {...register(
                                      `optionSubFee.${index}.Payfor`,
                                      Validate.Payfor
                                    )}
                                  >
                                    {listsfPayfor &&
                                      listsfPayfor.map((val) => {
                                        return (
                                          <option
                                            value={val.payForId}
                                            key={val.payForId}
                                          >
                                            {val.pfName}
                                          </option>
                                        );
                                      })}
                                  </select>
                                  {errors.optionSubFee?.[index]?.Payfor && (
                                    <span className="text-danger">
                                      {
                                        errors.optionSubFee?.[index]?.Payfor
                                          .message
                                      }
                                    </span>
                                  )}
                                </div>
                              </div>
                              <div className="col-sm-2">
                                <div className="form-group">
                                  <label>Đơn Giá</label>
                                  <input
                                    type="text"
                                    className="form-control"
                                    placeholder="Đơn Giá"
                                    id="DonGia"
                                    {...register(
                                      `optionSubFee.${index}.DonGia`,
                                      Validate.DonGia
                                    )}
                                  />
                                  {errors.optionSubFee?.[index]?.DonGia && (
                                    <span className="text-danger">
                                      {
                                        errors.optionSubFee?.[index]?.DonGia
                                          .message
                                      }
                                    </span>
                                  )}
                                </div>
                              </div>
                              <div className="col-sm-3">
                                <div className="form-group">
                                  <label>Ghi Chú</label>
                                  <input
                                    type="text"
                                    className="form-control"
                                    placeholder="Ghi Chú"
                                    id="GhiChu"
                                    {...register(
                                      `optionSubFee.${index}.GhiChu`,
                                      Validate.GhiChu
                                    )}
                                  />
                                  {errors.optionSubFee?.[index]?.GhiChu && (
                                    <span className="text-danger">
                                      {
                                        errors.optionSubFee?.[index]?.GhiChu
                                          .message
                                      }
                                    </span>
                                  )}
                                </div>
                              </div>
                            </div>
                          </td>
                          <td>
                            <div className="form-group">
                              {index >= 1 && (
                                <button
                                  type="button"
                                  className="btn btn-title btn-sm btn-default mx-1"
                                  gloss="Xóa Dòng"
                                  onClick={() => remove(index)}
                                >
                                  <i className="fas fa-minus"></i>
                                </button>
                              )}
                            </div>
                          </td>
                        </tr>
                      ))}
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
        </TabPanel>
        <TabPanel>
          <>
            <div className="card card-primary">
              <section className="content">
                <div className="card">
                  <div className="card-body">
                    <div
                      className="container-datatable"
                      style={{ height: "50vm" }}
                    >
                      <DataTable
                        columns={columns}
                        data={data}
                        progressPending={IsLoading}
                        highlightOnHover
                      />
                    </div>
                  </div>
                  <div className="card-footer"></div>
                </div>
                <div
                  className="modal fade"
                  id="modal-xl"
                  data-backdrop="static"
                  ref={parseExceptionModal}
                  aria-labelledby="parseExceptionModal"
                  backdrop="static"
                >
                  <div
                    className="modal-dialog modal-dialog-scrollable"
                    style={{ maxWidth: "90%" }}
                  >
                    <div className="modal-content">
                      <div className="modal-header">
                        <button
                          type="button"
                          className="close"
                          data-dismiss="modal"
                          onClick={() => hideModal()}
                          aria-label="Close"
                        >
                          <span aria-hidden="true">×</span>
                        </button>
                      </div>
                      <div className="modal-body">
                        {/* {ShowModal === "ShowImage" && (
                  <div>
                    <img
                      src={image}
                      style={{ margin: "auto", left: "0", right: "0" }}
                    />
                  </div>
                )} */}
                      </div>
                    </div>
                  </div>
                </div>
              </section>
            </div>
          </>
        </TabPanel>
      </Tabs>
    </>
  );
};

export default AddSubFeeByHandling;
