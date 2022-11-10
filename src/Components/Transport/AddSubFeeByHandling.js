import { useState, useEffect, useRef, useMemo } from "react";
import { getData, postData } from "../Common/FuncAxios";
import { useForm, Controller, useFieldArray } from "react-hook-form";
import { Tab, Tabs, TabList, TabPanel } from "react-tabs";
import Select from "react-select";
import DataTable from "react-data-table-component";
import moment from "moment";
import { Modal } from "bootstrap";

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

  const Validate = {};

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
            DonGia: null,
            GhiChu: null,
          },
        ]);
      }
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
      //   name: "Xem Hình",
      cell: (val) => (
        <button
          onClick={() => handleEditButtonClick(val, SetShowModal("Update"))}
          type="button"
          className="btn btn-sm btn-default"
        >
          <i className="far fa-edit"></i>
        </button>
      ),
      ignoreRowClick: true,
      allowOverflow: true,
      button: true,
    },

    {
      selector: (row) => row.id,
      omit: true,
    },
    {
      //   name: "Tên Hình",
      selector: (row) => row.fileName,
      sortable: true,
    },
    {
      name: "",
      selector: (row) => row.fileType,
      sortable: true,
    },
    {
      //   name: "Thời Gian Upload",
      selector: (row) => moment(row.uploadedTime).format("DD/MM/YYYY HH:mm:ss"),
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
      `SubFeePrice/GetListSubFeeIncurredByHandling?id=${maDieuPhoi}`
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
      if (val.IsApprove === false) {
        arr.push({
          MaPhuPhi: val.MaPhuPhi.value,
          DonGia: val.DonGia,
          GhiChu: val.GhiChu,
        });
      }
    });

    // const createPriceTable = await postData("PriceTable/CreatePriceTable", arr);
    // if (createPriceTable === 1) {
    //   reset();
    // }

    SetIsLoading(false);
  };

  const handleResetClick = () => {
    reset();
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
          <Tab>Danh sách phụ phí phát sinh</Tab>
        </TabList>

        <TabPanel>
          <div className="card card-primary">
            <div className="card-header">
              <h3 className="card-title">Form Thêm phụ phí phát sinh</h3>
            </div>
            <div>{IsLoading === true && <div>Loading...</div>}</div>

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
                              <div className="col-sm-7">
                                <div className="form-group">
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
                                  className="btn btn-sm btn-default"
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
              <div className="card-header">
                <h3 className="card-title">Danh sách phụ phí phát sinh</h3>
              </div>
              <div>{IsLoading === true && <div>Loading...</div>}</div>
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
