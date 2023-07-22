import { useState, useEffect, useMemo } from "react";
import {
  getData,
  getDataCustom,
  getFile,
  postData,
  postFile,
} from "../Common/FuncAxios";
import { useForm, Controller } from "react-hook-form";
import DataTable from "react-data-table-component";
import moment from "moment";
import Select from "react-select";
import { Tooltip, OverlayTrigger } from "react-bootstrap";
import { ToastError } from "../Common/FuncToast";
import ConfirmDialog from "../Common/Dialog/ConfirmDialog";
import { useRef } from "react";
import { Modal } from "bootstrap";
import DatePicker from "react-datepicker";
import CreateSubfeeLolo from "./CreateSubfeeLolo";
import UpdateSubfeeLolo from "./UpdateSubfeeLolo";

const customStyles = {
  control: (provided, state) => ({
    ...provided,
    background: "#fff",
    borderColor: "#9e9e9e",
    minHeight: "30px",
    height: "30px",
    boxShadow: state.isFocused ? null : null,
  }),

  valueContainer: (provided, state) => ({
    ...provided,
    height: "30px",
    padding: "0 6px",
  }),

  input: (provided, state) => ({
    ...provided,
    margin: "0px",
  }),
  indicatorSeparator: (state) => ({
    display: "none",
  }),
  indicatorsContainer: (provided, state) => ({
    ...provided,
    height: "30px",
  }),
};

const SubfeeLoloPage = () => {
  const {
    control,
    setValue,
    formState: { errors },
  } = useForm({
    mode: "onChange",
  });

  const columns = useMemo(() => [
    {
      cell: (val) =>
        val.trangThai === 51 ? (
          <>
            <button
              onClick={() =>
                handleEditButtonClick(
                  val,
                  SetShowModal("EditSubfeeLolo"),
                  setTitle("Cập Nhật Phụ Phí Lolo")
                )
              }
              type="button"
              className="btn btn-title btn-sm btn-default mx-1"
              gloss="Chỉnh Sửa"
            >
              <i className="far fa-edit"></i>
            </button>
          </>
        ) : (
          <>
            <button
              disabled={true}
              type="button"
              className="btn btn-title btn-sm btn-default mx-1"
              gloss="Chỉnh Sửa"
            >
              <i className="far fa-edit"></i>
            </button>
          </>
        ),
    },
    {
      selector: (row) => row.id,
      sortable: true,
      omit: true,
    },
    {
      name: "TrangThai",
      selector: (row) => row.trangThai,
      omit: true,
    },
    {
      name: <div>Trạng Thái</div>,
      selector: (row) => row.tenTrangThai,
    },
    {
      name: <div>Mã Đối Tác</div>,
      selector: (row) => row.maKh,
      sortable: true,
    },
    {
      name: <div>Tên Đối Tác</div>,
      selector: (row) => row.tenKh,
      sortable: true,
    },
    {
      name: <div>Hãng Tàu</div>,
      selector: (row) => row.hangTau,
      sortable: true,
    },
    {
      name: <div>Điểm Nâng / Hạ</div>,
      selector: (row) => (
        <OverlayTrigger
          placement="top"
          overlay={
            <Tooltip id="tooltip">
              <strong>{row.tenDiaDiem}</strong>
            </Tooltip>
          }
        >
          <div bsStyle="default">{row.tenDiaDiem}</div>
        </OverlayTrigger>
      ),
    },
    {
      name: "Đơn Giá",
      selector: (row) =>
        row.donGia.toLocaleString("vi-VI", {
          style: "currency",
          currency: "VND",
        }),
    },
    {
      name: <div>Loại Phụ Phí</div>,
      selector: (row) => row.loaiPhuPhi,
      sortable: true,
    },
    {
      name: <div>Loại Phương Tiện</div>,
      selector: (row) => row.loaiCont,
      sortable: true,
    },
    {
      name: <div>Ngày Áp Dụng</div>,
      selector: (row) => (
        <div className="text-wrap">
          {moment(row.ngayApDung).format("DD/MM/YYYY")}
        </div>
      ),
      sortable: true,
    },
    {
      name: <div>Người Duyệt</div>,
      selector: (row) => row.approver,
      sortable: true,
    },
    {
      name: <div>Ngày Hết Hiệu Lực</div>,
      selector: (row) => (!row.ngayHetHieuLuc ? "" : row.ngayHetHieuLuc),
      sortable: true,
    },
  ]);

  const [page, setPage] = useState(1);
  const [IsLoading, SetIsLoading] = useState(false);
  const [data, setData] = useState([]);
  const [totalRows, setTotalRows] = useState(0);
  const [perPage, setPerPage] = useState(30);
  const [keySearch, setKeySearch] = useState("");
  const [selectIdClick, setSelectIdClick] = useState({});

  const [listVehicleType, setListVehicleType] = useState([]);
  const [vehicleType, setVehicleType] = useState("");
  const [listPlace, setListPlace] = useState([]);
  const [placeSelected, setPlaceSelected] = useState("");
  const [listShip, setListShip] = useState([]);
  const [shipSelected, setShipSelected] = useState("");
  const [listCustomer, setListCustomer] = useState([]);
  const [customerSelected, setCustomerSelected] = useState("");

  const [listStatus, setListStatus] = useState([]);
  const [statusSelected, setStatusSelected] = useState("");

  const [fromDate, setFromDate] = useState("");
  const [toDate, setToDate] = useState("");

  const [titleConfirmBox, setTitleConfirmBox] = useState("");
  const [ShowConfirm, setShowConfirm] = useState(false);
  const [funcName, setFuncName] = useState("");
  const [isAccept, setIsAccept] = useState();

  const [ShowModal, SetShowModal] = useState("");
  const [title, setTitle] = useState("");
  const [modal, setModal] = useState(null);
  const parseExceptionModal = useRef();

  const [selectedRows, setSelectedRows] = useState([]);
  const [toggledClearRows, setToggleClearRows] = useState(false);

  useEffect(() => {
    (async () => {
      let getStatusList = await getDataCustom(`Common/GetListStatus`, [
        "SubfeeLolo",
      ]);
      setListStatus(getStatusList);

      const getListPlace = await getData(
        "Address/GetListAddressSelect?pointType=&type="
      );

      let arrPlace = [];
      arrPlace.push({ value: "", label: "Chọn Địa Điểm" });
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
        obj.push({ value: "", label: "Chọn Khách Hàng" });
        getListCustomer.map((val) => {
          obj.push({
            value: val.maKh,
            label: val.maKh + " - " + val.tenKh,
          });
        });
        setListCustomer(obj);
      }

      let getListShip = await getData("Common/GetListShipping");
      setListShip(getListShip);

      let getlistVehicleType = await getData(`Common/GetListVehicleType`);
      setListVehicleType(getlistVehicleType);
    })();
  }, []);

  useEffect(() => {
    if (
      (listVehicleType.length > 0 &&
        listPlace.length > 0 &&
        listCustomer.length > 0,
      listStatus.length > 0)
    ) {
      fetchData(1);
    }
  }, [listVehicleType, listPlace, listCustomer, listStatus]);

  const showModalForm = () => {
    const modal = new Modal(parseExceptionModal.current, {
      keyboard: false,
      backdrop: "static",
    });
    setModal(modal);
    modal.show();
  };

  const hideModal = () => {
    modal.hide();
  };

  const handlePerRowsChange = async (newPerPage, page) => {
    SetIsLoading(true);

    let fromDay = fromDate === "" ? "" : moment(fromDate).format("YYYY-MM-DD");
    let toDay = toDate === "" ? "" : moment(toDate).format("YYYY-MM-DD");

    const dataCus = await getData(
      `SubfeeLolo/GetListSubfeeLolo?PageNumber=${page}&PageSize=${newPerPage}&KeyWord=${keySearch}&vehicleType=${vehicleType}&placeId=${placeSelected}&customerId=${customerSelected}&statusId=${statusSelected}&fromDate=${fromDay}&toDate=${toDay}`
    );
    setData(dataCus.data);
    setPerPage(newPerPage);
    setTotalRows(dataCus.totalRecords);
    SetIsLoading(false);
  };

  const fetchData = async (
    page,
    KeyWord = "",
    vehicleType = "",
    placeSelected = "",
    customerSelected = "",
    statusSelected = "",
    fromDate = "",
    toDate = ""
  ) => {
    SetIsLoading(true);

    fromDate = fromDate === "" ? "" : moment(fromDate).format("YYYY-MM-DD");
    toDate = toDate === "" ? "" : moment(toDate).format("YYYY-MM-DD");

    const dataCus = await getData(
      `SubfeeLolo/GetListSubfeeLolo?PageNumber=${page}&PageSize=${perPage}&KeyWord=${KeyWord}&vehicleType=${vehicleType}&placeId=${placeSelected}&customerId=${customerSelected}&statusId=${statusSelected}&fromDate=${fromDate}&toDate=${toDate}`
    );
    setData(dataCus.data);
    setTotalRows(dataCus.totalRecords);
    SetIsLoading(false);
  };

  const handlePageChange = async (page) => {
    setPage(page);
    await fetchData(
      page,
      keySearch,
      vehicleType,
      placeSelected,
      customerSelected,
      statusSelected,
      fromDate,
      toDate
    );
  };

  const handleOnChangeVehicleType = async (value) => {
    setVehicleType(value);
    await fetchData(
      page,
      keySearch,
      value,
      placeSelected,
      customerSelected,
      statusSelected,
      fromDate,
      toDate
    );
  };

  const handleOnchangePlace = async (value) => {
    setPlaceSelected(value);
    await fetchData(
      page,
      keySearch,
      vehicleType,
      value,
      customerSelected,
      statusSelected,
      fromDate,
      toDate
    );
  };

  const handleOnChangeCustomer = async (value) => {
    setCustomerSelected(value);
    await fetchData(
      page,
      keySearch,
      vehicleType,
      placeSelected,
      value,
      statusSelected,
      fromDate,
      toDate
    );
  };

  const handleOnChangeStatus = async (value) => {
    setStatusSelected(value);
    await fetchData(
      page,
      keySearch,
      vehicleType,
      placeSelected,
      customerSelected,
      value,
      fromDate,
      toDate
    );
  };

  const handleRefeshDataClick = async () => {
    await fetchData(1);
    setKeySearch("");
    setVehicleType("");
    setPlaceSelected("");
    setShipSelected("");
    setCustomerSelected("");
    setStatusSelected("");
    setFromDate("");
    setToDate("");
    setValue("placeSelected", null);
    setValue("customerSelected", null);
  };

  const handleSearchClick = async () => {
    await fetchData(
      page,
      keySearch,
      vehicleType,
      placeSelected,
      customerSelected,
      statusSelected,
      fromDate,
      toDate
    );
  };

  const handleEditButtonClick = async (value) => {
    setSelectIdClick(value);
    showModalForm();
  };

  const handleChange = (state) => {
    setSelectedRows(state.selectedRows);
  };

  const handleClearRows = () => {
    setToggleClearRows(!toggledClearRows);
    setSelectedRows([]);
  };

  const ShowConfirmDialog = (funcName) => {
    setFuncName(funcName);
    setShowConfirm(true);
  };

  const funcAgree = async () => {
    if (funcName) {
      switch (funcName) {
        case "ApproveSubfeeLolo":
          if (selectedRows && selectedRows.length > 0) {
            await AcceptSubFee(isAccept);
          }
          break;
        default:
          break;
      }
    }
  };

  const AcceptSubFee = async (isAccept) => {
    if (
      selectedRows &&
      selectedRows.length > 0 &&
      Object.keys(selectedRows).length > 0
    ) {
      let arr = [];
      selectedRows.map((val) => {
        arr.push({
          ID: val.id,
          isApprove: isAccept,
        });
      });

      const SetApprove = await postData(`SubfeeLolo/ApproveSubfeeLolo`, {
        Result: arr,
      });

      handleClearRows();
      setShowConfirm(false);

      if (SetApprove === 1) {
        handleSearchClick();
      }
    }
  };

  const handleExportExcel = async () => {
    SetIsLoading(true);
    let timeStart =
      fromDate === "" ? "" : moment(fromDate).format("YYYY-MM-DD");
    let timeEnd = toDate === "" ? "" : moment(toDate).format("YYYY-MM-DD");
    const getFileDownLoad = await getFile(
      `SubfeeLolo/ExportExcelLoloSubfee?PageNumber=${1}&PageSize=${10000}&KeyWord=${keySearch}&vehicleType=${vehicleType}&placeId=${placeSelected}&customerId=${customerSelected}&statusId=${statusSelected}&fromDate=${timeStart}&toDate=${timeEnd}`,
      "ListSubfeeLolo" + moment(new Date()).format("DD/MM/YYYY")
    );
    SetIsLoading(false);
  };

  const handleExcelImportClick = async (e) => {
    SetIsLoading(true);
    var file = e.target.files[0];
    e.target.value = null;

    const importExcelCus = await postFile(
      "SubfeeLolo/ReadFileExcelLoloSubfee",
      {
        formFile: file,
      }
    );

    if (importExcelCus === 1) {
      await fetchData(1);
    }

    SetIsLoading(false);
  };

  const handleExportTemplateExcel = async () => {
    let download = await getFile(
      `SubfeeLolo/ExportTemplateExcel`,
      "TemplateSubfeeLolo" + moment(new Date()).format("DD/MM/YYYY")
    );
  };

  return (
    <div>
      <section className="content-header">
        <div className="container-fluid">
          <div className="row mb-2">
            <div className="col-sm-6">
              <h1>Quản Lý Phụ Phí LoLo</h1>
            </div>
            {/* <div className="col-sm-6">
              <ol className="breadcrumb float-sm-right">
                <li className="breadcrumb-item">
                  <a href="#">Home</a>
                </li>
                <li className="breadcrumb-item active">Blank Page</li>
              </ol>
            </div> */}
          </div>
        </div>
      </section>

      <div className="card card-default">
        <div className="card-header">
          {/* <h3 className="card-title">{title}</h3> */}
          <div className="container-fruid">
            <div className="row">
              <div className="col-sm-3">
                <button
                  type="button"
                  className="btn btn-title btn-sm btn-default mx-1"
                  gloss="Thêm Mới Khách Hàng"
                  onClick={() =>
                    showModalForm(
                      SetShowModal("Create"),
                      setTitle("Tạo Mới Thông Tin Đối Tác")
                    )
                  }
                >
                  <i className="fas fa-plus-circle"></i>
                </button>
                <button
                  type="button"
                  className="btn btn-title btn-sm btn-default"
                  gloss="Duyệt Phụ Phí"
                  onClick={() => {
                    setTitleConfirmBox("Duyệt Phụ Phí");
                    ShowConfirmDialog("ApproveSubfeeLolo");
                    setIsAccept(0);
                  }}
                >
                  <i className="fas fa-thumbs-up"></i>
                </button>
                <button
                  type="button"
                  className="btn btn-title btn-sm btn-default mx-1"
                  gloss="Không Duyệt Phụ Phí"
                  onClick={() => {
                    setTitleConfirmBox("Không Duyệt Phụ Phí");
                    ShowConfirmDialog("ApproveSubfeeLolo");
                    setIsAccept(1);
                  }}
                >
                  <i className="fas fa-thumbs-down"></i>
                </button>
              </div>
              <div className="col col-sm">
                <div className="row">
                  <div className="col col-sm">
                    <div className="form-group">
                      <Controller
                        name="placeSelected"
                        control={control}
                        render={({ field }) => (
                          <Select
                            {...field}
                            className="basic-multi-select"
                            classNamePrefix={"form-control"}
                            value={field.value}
                            options={listPlace}
                            styles={customStyles}
                            onChange={(field) => {
                              setValue("placeSelected", field);
                              handleOnchangePlace(field.value);
                            }}
                            placeholder="Nơi Nâng/Hạ"
                          />
                        )}
                      />
                    </div>
                  </div>
                  <div className="col col-sm">
                    <div className="form-group">
                      <Controller
                        name="customerSelected"
                        control={control}
                        render={({ field }) => (
                          <Select
                            {...field}
                            className="basic-multi-select"
                            classNamePrefix={"form-control"}
                            value={field.value}
                            options={listCustomer}
                            styles={customStyles}
                            onChange={(field) => {
                              setValue("customerSelected", field);
                              handleOnChangeCustomer(field.value);
                            }}
                            placeholder="Khách Hàng"
                          />
                        )}
                      />
                    </div>
                  </div>
                </div>
              </div>
              <div className="col col-sm">
                <div className="row">
                  <div className="col col-sm">
                    <div className="input-group input-group-sm">
                      <select
                        className="form-control form-control-sm"
                        onChange={(e) =>
                          handleOnChangeVehicleType(e.target.value)
                        }
                        value={vehicleType}
                      >
                        <option value="">Phương tiện vận tải</option>
                        {listVehicleType &&
                          listVehicleType
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
                    </div>
                  </div>
                  <div className="col col-sm">
                    <div className="input-group input-group-sm">
                      <select
                        className="form-control form-control-sm"
                        onChange={(e) => handleOnChangeStatus(e.target.value)}
                        value={statusSelected}
                      >
                        <option value="">Trạng Thái</option>
                        {listStatus &&
                          listStatus.map((val) => {
                            return (
                              <option value={val.statusId} key={val.statusId}>
                                {val.statusContent}
                              </option>
                            );
                          })}
                      </select>
                    </div>
                  </div>
                </div>
              </div>

              <div className="col col-sm">
                <div className="row">
                  <div className="col col-sm">
                    <div className="input-group input-group-sm">
                      <DatePicker
                        selected={fromDate}
                        onChange={(date) => setFromDate(date)}
                        dateFormat="dd/MM/yyyy"
                        className="form-control form-control-sm"
                        placeholderText="Từ ngày"
                        value={fromDate}
                      />
                    </div>
                  </div>
                  <div className="col col-sm">
                    <div className="input-group input-group-sm">
                      <DatePicker
                        selected={toDate}
                        onChange={(date) => setToDate(date)}
                        dateFormat="dd/MM/yyyy"
                        className="form-control form-control-sm"
                        placeholderText="Đến Ngày"
                        value={toDate}
                      />
                    </div>
                  </div>
                </div>
              </div>
              <div className="col col-sm ">
                <div className="input-group input-group-sm">
                  <input
                    type="text"
                    className="form-control"
                    value={keySearch}
                    onChange={(e) => setKeySearch(e.target.value)}
                  />
                  <span className="input-group-append">
                    <button
                      type="button"
                      className="btn btn-sm btn-default"
                      onClick={() => handleSearchClick()}
                    >
                      <i className="fas fa-search"></i>
                    </button>
                  </span>
                  <button
                    type="button"
                    className="btn btn-sm btn-default mx-2"
                    onClick={() => handleRefeshDataClick()}
                  >
                    <i className="fas fa-sync-alt"></i>
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
        <div>
          <div className="card-body">
            <div className="container-datatable" style={{ height: "50vm" }}>
              <DataTable
                columns={columns}
                data={data}
                progressPending={IsLoading}
                pagination
                paginationServer
                paginationTotalRows={totalRows}
                paginationRowsPerPageOptions={[30, 50, 100]}
                onChangeRowsPerPage={handlePerRowsChange}
                onChangePage={handlePageChange}
                selectableRows
                onSelectedRowsChange={handleChange}
                clearSelectedRows={toggledClearRows}
                highlightOnHover
                striped
                direction="auto"
                responsive
                dense
                fixedHeader
                fixedHeaderScrollHeight="60vh"
              />
            </div>
          </div>
          <div className="card-footer">
            <div className="row">
              <div className="col-sm-3">
                <button
                  type="button"
                  className="btn btn-title btn-sm btn-default mx-1"
                  gloss="Tải Bảng Phụ Phí Lolo"
                  onClick={() => handleExportExcel()}
                >
                  <i className="fas fa-file-excel"></i>
                </button>
                <div className="upload-btn-wrapper">
                  <button className="btn btn-sm btn-default mx-1">
                    <i className="fas fa-upload"></i>
                  </button>
                  <input
                    type="file"
                    name="myfile"
                    onChange={(e) => handleExcelImportClick(e)}
                  />
                </div>
                <button
                  type="button"
                  className="btn btn-title btn-sm btn-default mx-1"
                  gloss="Tải Template Phụ Phí Lolo"
                  onClick={() => handleExportTemplateExcel()}
                >
                  <i className="fas fa-file-download"></i>
                </button>
                {/* <a
                    href={FileExcelImport}
                    download="TemplateImportPriceTable.xlsx"
                    className="btn btn-sm btn-default mx-1"
                  >
                    <i className="fas fa-file-download"></i>
                  </a> */}
              </div>
            </div>
          </div>
        </div>
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
          style={{ maxWidth: "95%" }}
        >
          <div className="modal-content">
            <div className="modal-header">
              <h5>{title}</h5>
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
              <>
                {ShowModal === "Create" && (
                  <CreateSubfeeLolo refeshData={handleSearchClick} />
                )}
                {ShowModal === "EditSubfeeLolo" && (
                  <UpdateSubfeeLolo
                    selectIdClick={selectIdClick}
                    refeshData={handleSearchClick}
                    hideModal={hideModal}
                  />
                )}
              </>
            </div>
          </div>
        </div>
      </div>
      {ShowConfirm === true && (
        <ConfirmDialog
          setShowConfirm={setShowConfirm}
          title={titleConfirmBox}
          content={
            "Khi thực hiện hành động này sẽ không thể hoàn tác lại được nữa."
          }
          funcAgree={funcAgree}
        />
      )}
    </div>
  );
};

export default SubfeeLoloPage;
