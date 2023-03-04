import { useMemo, useState, useEffect, useRef } from "react";
import { getData, getDataCustom, postData } from "../Common/FuncAxios";
import DataTable from "react-data-table-component";
import moment from "moment";
import { Modal } from "bootstrap";
import DatePicker from "react-datepicker";
import CreateSubFee from "./CreateSubFee";
import ApproveSubFee from "./ApproveSubFee";
import ConfirmDialog from "../Common/Dialog/ConfirmDialog";
import { ToastError } from "../Common/FuncToast";

const SubFeePage = () => {
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(false);
  const [totalRows, setTotalRows] = useState(0);
  const [perPage, setPerPage] = useState(10);
  const [keySearch, setKeySearch] = useState("");

  const [ShowModal, SetShowModal] = useState("");
  const [modal, setModal] = useState(null);
  const parseExceptionModal = useRef();

  const [fromDate, setFromDate] = useState("");
  const [toDate, setToDate] = useState("");
  const [toggledClearRows, setToggleClearRows] = useState(false);
  const [selectedRows, setSelectedRows] = useState([]);
  const [selectIdClick, setSelectIdClick] = useState({});
  const [listStatus, setListStatus] = useState([]);
  const [status, setStatus] = useState("");

  const [ShowConfirm, setShowConfirm] = useState(false);
  const [functionSubmit, setFunctionSubmit] = useState("");

  const [title, setTitle] = useState("");

  const columns = useMemo(() => [
    {
      name: <div>Mã phụ phí</div>,
      selector: (row) => row.priceId,
      sortable: true,
    },
    {
      name: <div>Loại Phụ Phí</div>,
      selector: (row) => <div className="text-wrap">{row.sfName}</div>,
      sortable: true,
    },
    {
      name: <div>Khách Hàng</div>,
      selector: (row) => <div className="text-wrap">{row.customerName}</div>,
      sortable: true,
    },
    {
      name: <div>Mã Hợp Đồng</div>,
      selector: (row) => <div className="text-wrap">{row.contractId}</div>,
      sortable: true,
    },
    {
      name: <div>Tên Hợp Đồng</div>,
      selector: (row) => <div className="text-wrap">{row.contractName}</div>,
    },
    {
      name: <div>Loại Phương Tiện</div>,
      selector: (row) => <div className="text-wrap">{row.vehicleType}</div>,
    },
    {
      name: "Cung Đường",
      selector: (row) => <div className="text-wrap">{row.tripName}</div>,
    },
    {
      name: <div>Khu Vực Lấy/Trả Rỗng</div>,
      selector: (row) => <div className="text-wrap">{row.areaName}</div>,
    },
    {
      name: <div>Loại Hàng Hóa</div>,
      selector: (row) => row.goodsType,
    },
    {
      name: <div>Đơn Giá</div>,
      selector: (row) =>
        row.unitPrice.toLocaleString("vi-VI", {
          style: "currency",
          currency: "VND",
        }),
    },
    {
      name: <div>Thời gian Áp Dụng</div>,
      selector: (row) => row.approvedDate,
      sortable: true,
    },
    {
      name: <div>Thời gian Hết Hiệu Lực</div>,
      selector: (row) => row.deactiveDate,
      sortable: true,
    },
    {
      name: <div>Trạng Thái</div>,
      selector: (row) => row.status,
      sortable: true,
    },
    {
      name: <div>Người Duyệt</div>,
      selector: (row) => row.approver,
    },
    {
      name: <div>Thời gian Tạo</div>,
      selector: (row) => (
        <div className="text-wrap">
          {moment(row.createdTime).format("DD-MM-YYYY HH:mm:ss")}
        </div>
      ),
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

  const hideModal = () => {
    modal.hide();
  };

  const handleChange = (state) => {
    setSelectedRows(state.selectedRows);
  };

  const handleClearRows = () => {
    setToggleClearRows(!toggledClearRows);
  };

  const fetchData = async (
    page,
    KeyWord = "",
    fromDate = "",
    toDate = "",
    status = ""
  ) => {
    setLoading(true);

    if (KeyWord !== "") {
      KeyWord = keySearch;
    }
    fromDate = fromDate === "" ? "" : moment(fromDate).format("YYYY-MM-DD");
    toDate = toDate === "" ? "" : moment(toDate).format("YYYY-MM-DD");
    const dataCus = await getData(
      `SubFeePrice/GetListSubFeePrice?PageNumber=${page}&PageSize=${perPage}&KeyWord=${KeyWord}&fromDate=${fromDate}&toDate=${toDate}&statusId=${status}`
    );

    formatTable(dataCus.data);
    setTotalRows(dataCus.totalRecords);
    setLoading(false);
  };

  const handlePageChange = async (page) => {
    await fetchData(page, keySearch, fromDate, toDate, status);
  };

  const handlePerRowsChange = async (newPerPage, page) => {
    setLoading(true);

    const dataCus = await getData(
      `SubFeePrice/GetListSubFeePrice?PageNumber=${page}&PageSize=${newPerPage}&KeyWord=${keySearch}&fromDate=${fromDate}&toDate=${toDate}&statusId=${status}`
    );
    setPerPage(newPerPage);
    formatTable(dataCus.data);
    setTotalRows(dataCus.totalRecords);
    setLoading(false);
  };

  useEffect(() => {
    setLoading(true);
    (async () => {
      let getStatusList = await getDataCustom(`Common/GetListStatus`, [
        "SubFee",
      ]);
      setListStatus(getStatusList);
    })();

    fetchData(1);
    setLoading(false);
  }, []);

  function formatTable(data) {
    data.map((val) => {
      !val.approvedDate
        ? (val.approvedDate = "")
        : (val.approvedDate = moment(val.approvedDate).format("DD/MM/YYYY"));
      !val.deactiveDate
        ? (val.deactiveDate = "")
        : (val.deactiveDate = moment(val.deactiveDate).format("DD/MM/YYYY"));
    });
    setData(data);
  }

  const ShowConfirmDialog = () => {
    if (selectedRows.length < 1) {
      ToastError("Vui lòng chọn phụ phí trước đã");
      return;
    } else {
      setShowConfirm(true);
    }
  };

  const DisableSubFee = async () => {
    if (
      selectedRows &&
      selectedRows.length > 0 &&
      Object.keys(selectedRows).length > 0
    ) {
      let arr = [];
      selectedRows.map((val) => {
        arr.push(val.priceId);
      });

      const SetApprove = await postData(`SubFeePrice/DisableSubFeePrice`, arr);

      if (SetApprove === 1) {
        fetchData(1);
      }
      setSelectedRows([]);
      handleClearRows();
      setShowConfirm(false);
    }
  };

  const DeleteSubFee = async () => {
    if (
      selectedRows &&
      selectedRows.length > 0 &&
      Object.keys(selectedRows).length > 0
    ) {
      let arr = [];
      selectedRows.map((val) => {
        arr.push(val.priceId);
      });

      const SetApprove = await postData(`SubFeePrice/DeleteSubFeePrice`, arr);

      if (SetApprove === 1) {
        fetchData(1);
      }
      setSelectedRows([]);
      handleClearRows();
      setShowConfirm(false);
    }
  };

  const funcAgree = () => {
    if (functionSubmit && functionSubmit.length > 0) {
      switch (functionSubmit) {
        case "Disable":
          return DisableSubFee();
        case "Delete":
          return DeleteSubFee();
      }
    }
  };

  const handleSearchClick = () => {
    fetchData(1, keySearch, fromDate, toDate, status);
  };

  const handleOnChangeStatus = (value) => {
    setLoading(true);
    setStatus(value);
    fetchData(1, keySearch, fromDate, toDate, value);
    setLoading(false);
  };

  const handleRefeshDataClick = () => {
    setKeySearch("");
    setFromDate("");
    setToDate("");
    setPerPage(10);
    fetchData(1);
  };

  return (
    <>
      <section className="content-header">
        <div className="container-fluid">
          <div className="row mb-2">
            <div className="col-sm-6">
              <h1>Quản Lý Phụ Phí Theo Hợp Đồng</h1>
            </div>
          </div>
        </div>
      </section>

      <section className="content">
        <div className="card">
          <div className="card-header">
            <div className="container-fruid">
              <div className="row">
                <div className="col col-sm">
                  <button
                    type="button"
                    className="btn btn-title btn-sm btn-default mx-1"
                    gloss="Thêm Mới Phụ Phí"
                    onClick={() =>
                      showModalForm(
                        SetShowModal("Create"),
                        setSelectIdClick({}),
                        setTitle("Tạo Mới Phụ Phí Theo Hợp Đồng")
                      )
                    }
                  >
                    <i className="fas fa-plus-circle"></i>
                  </button>
                  <button
                    type="button"
                    className="btn btn-title btn-sm btn-default mx-1"
                    gloss="Duyệt Phụ Phí"
                    onClick={() =>
                      showModalForm(
                        SetShowModal("ApproveSubFee"),
                        setSelectIdClick({}),
                        setTitle("Duyệt Phụ Phí Theo Hợp Đồng")
                      )
                    }
                  >
                    <i className="fas fa-check-double"></i>
                  </button>
                  <button
                    type="button"
                    className="btn btn-title btn-sm btn-default mx-1"
                    gloss="Vô Hiệu Phụ Phí"
                    onClick={() => {
                      setFunctionSubmit("Disable");
                      ShowConfirmDialog();
                    }}
                  >
                    <i className="fas fa-eye-slash"></i>
                  </button>
                  <button
                    type="button"
                    className="btn btn-title btn-sm btn-default mx-1"
                    gloss="Xóa Phụ Phí"
                    onClick={() => {
                      setFunctionSubmit("Delete");
                      ShowConfirmDialog();
                    }}
                  >
                    <i className="fas fa-trash-alt"></i>
                  </button>
                </div>
                <div className="col col-sm">
                  <div className="row">
                    <div className="col col-sm"></div>
                    <div className="col col-sm">
                      <div className="input-group input-group-sm">
                        <select
                          className="form-control form-control-sm"
                          onChange={(e) => handleOnChangeStatus(e.target.value)}
                          value={status}
                        >
                          <option value="">Tất Cả Trạng Thái</option>
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
          <div className="card-body">
            <div className="container-datatable" style={{ height: "50vm" }}>
              <DataTable
                title="Danh sách Phụ Phí"
                columns={columns}
                data={data}
                progressPending={loading}
                pagination
                paginationServer
                selectableRows
                paginationRowsPerPageOptions={[10, 30, 50, 100]}
                clearSelectedRows={toggledClearRows}
                paginationTotalRows={totalRows}
                onSelectedRowsChange={handleChange}
                onChangeRowsPerPage={handlePerRowsChange}
                onChangePage={handlePageChange}
                highlightOnHover
                striped
                direction="auto"
                responsive
                fixedHeader
                fixedHeaderScrollHeight="60vh"
              />
            </div>
          </div>
          <div className="card-footer">
            <div className="row"></div>
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
                    <CreateSubFee getListSubFee={fetchData} />
                  )}
                  {ShowModal === "ApproveSubFee" && (
                    <ApproveSubFee
                      getListSubFee={fetchData}
                      checkShowModal={modal}
                    />
                  )}
                </>
              </div>
            </div>
          </div>
        </div>
      </section>
      {ShowConfirm === true && (
        <ConfirmDialog
          setShowConfirm={setShowConfirm}
          title={"Bạn có chắc chắn với quyết định này?"}
          content={
            "Khi thực hiện hành động này sẽ không thể hoàn tác lại được nữa."
          }
          funcAgree={funcAgree}
        />
      )}
    </>
  );
};

export default SubFeePage;
