import { useMemo, useState, useEffect, useRef } from "react";
import { getData, postData } from "../Common/FuncAxios";
import DataTable from "react-data-table-component";
import moment from "moment";
import { Modal } from "bootstrap";
import { ToastError } from "../Common/FuncToast";
import DatePicker from "react-datepicker";
import UpdateSubFee from "./UpdateSubFee";
import ConfirmDialog from "../Common/Dialog/ConfirmDialog";

const ApproveSubFee = (props) => {
  const { getListSubFee, checkShowModal } = props;
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

  const [selectedRows, setSelectedRows] = useState([]);
  const [selectIdClick, setSelectIdClick] = useState({});
  const [toggledClearRows, setToggleClearRows] = useState(false);

  const [isAccept, setIsAccept] = useState();
  const [ShowConfirm, setShowConfirm] = useState(false);

  const columns = useMemo(() => [
    {
      cell: (val) => (
        <button
          title="Cập nhật"
          onClick={() => handleEditButtonClick(val, SetShowModal("Update"))}
          type="button"
          className="btn btn-title btn-sm btn-default mx-1"
          gloss="Cập Nhật Thông Tin"
        >
          <i className="far fa-edit"></i>
        </button>
      ),
      ignoreRowClick: true,
      allowOverflow: true,
      button: true,
    },
    {
      name: "Mã Phụ Phí",
      selector: (row) => row.priceId,
    },
    {
      name: "Loại Phụ Phí",
      selector: (row) => row.sfName,
      sortable: true,
    },
    {
      name: "Mã Hợp Đồng",
      selector: (row) => row.contractId,
      sortable: true,
    },
    {
      name: "Tên Hợp Đồng",
      selector: (row) => row.contractName,
    },
    {
      name: "Điểm 1",
      selector: (row) => row.firstPlace,
    },
    {
      name: "Điểm 2",
      selector: (row) => row.secondPlace,
    },
    {
      name: "Loại Hàng Hóa",
      selector: (row) => row.goodsType,
    },
    {
      name: "Đơn Giá",
      selector: (row) =>
        row.unitPrice.toLocaleString("vi-VI", {
          style: "currency",
          currency: "VND",
        }),
    },
    {
      name: "Trạng Thái",
      selector: (row) => row.status,
      sortable: true,
    },
    {
      name: "Thời gian Tạo",
      selector: (row) => moment(row.createdTime).format("DD-MM-YYYY HH:mm:ss"),
      sortable: true,
    },
  ]);

  useEffect(() => {
    if (checkShowModal && Object.keys(checkShowModal).length > 0) {
      fetchData(1);
    }
  }, [props, checkShowModal]);

  const handleEditButtonClick = async (val) => {
    if (val && Object.keys(val).length > 0) {
      let getById = await getData(
        `SubFeePrice/GetSubFeePriceById?id=${val.priceId}`
      );
      setSelectIdClick(getById);
      showModalForm();
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
          SubFeePriceId: val.priceId,
          Selection: isAccept,
        });
      });

      const SetApprove = await postData(`SubFeePrice/ApproveSubFeePrice`, arr);

      if (SetApprove === 1) {
        fetchData(1);
        getListSubFee(1);
      }
      setSelectedRows([]);
      handleClearRows();
      setShowConfirm(false);
    }
  };

  const funcAgree = () => {
    if (selectedRows && selectedRows.length > 0) {
      AcceptSubFee(isAccept);
    }
  };

  const ShowConfirmDialog = () => {
    if (selectedRows.length < 1) {
      ToastError("Vui lòng chọn phụ phí để duyệt");
      return;
    } else {
      setShowConfirm(true);
    }
  };

  const handleChange = (state) => {
    setSelectedRows(state.selectedRows);
  };

  const handleClearRows = () => {
    setToggleClearRows(!toggledClearRows);
  };

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

  const fetchData = async (page, KeyWord = "", fromDate = "", toDate = "") => {
    setLoading(true);

    if (KeyWord !== "") {
      KeyWord = keySearch;
    }
    fromDate = fromDate === "" ? "" : moment(fromDate).format("YYYY-MM-DD");
    toDate = toDate === "" ? "" : moment(toDate).format("YYYY-MM-DD");
    const dataCus = await getData(
      `SubFeePrice/GetListSubFeePrice?PageNumber=${page}&PageSize=${perPage}&KeyWord=${KeyWord}&fromDate=${fromDate}&toDate=${toDate}&statusId=13`
    );

    formatTable(dataCus.data);
    setTotalRows(dataCus.totalRecords);
    setLoading(false);
  };

  const handlePageChange = async (page) => {
    await fetchData(page);
  };

  const handlePerRowsChange = async (newPerPage, page) => {
    setLoading(true);

    const dataCus = await getData(
      `SubFeePrice/GetListSubFeePrice?PageNumber=${page}&PageSize=${newPerPage}&KeyWord=${keySearch}&fromDate=${fromDate}&toDate=${toDate}&statusId=13`
    );
    setPerPage(newPerPage);
    formatTable(dataCus.data);
    setTotalRows(dataCus.totalRecords);
    setLoading(false);
  };

  function formatTable(data) {
    data.map((val) => {
      val.ngayApDung = moment(val.ngayApDung).format("DD/MM/YYYY");
      val.ngayHetHieuLuc = moment(val.ngayHetHieuLuc).format("DD/MM/YYYY");
    });
    setData(data);
  }

  const handleSearchClick = () => {
    fetchData(1, keySearch, fromDate, toDate);
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
              <h1>Duyệt Phụ Phí</h1>
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
                    className="btn btn-title btn-sm btn-default "
                    gloss="Duyệt Phụ Phí"
                    onClick={() => {
                      ShowConfirmDialog();
                      setIsAccept(0);
                    }}
                  >
                    <i className="fas fa-thumbs-up"></i>
                  </button>

                  <button
                    type="button"
                    className="btn btn-title btn-sm btn-default mx-4"
                    gloss="Không Duyệt Phụ Phí"
                    onClick={() => {
                      ShowConfirmDialog();
                      setIsAccept(1);
                    }}
                  >
                    <i className="fas fa-thumbs-down"></i>
                  </button>
                </div>
                <div className="col col-sm"></div>
                <div className="col col-sm">
                  <div className="row">
                    <div className="col col-sm"></div>
                    <div className="col col-sm"></div>
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
                title="Danh Sách Phụ Phí Chờ Duyệt"
                columns={columns}
                data={data}
                progressPending={loading}
                pagination
                selectableRows
                paginationServer
                paginationTotalRows={totalRows}
                onSelectedRowsChange={handleChange}
                onChangeRowsPerPage={handlePerRowsChange}
                onChangePage={handlePageChange}
                clearSelectedRows={toggledClearRows}
                highlightOnHover
                striped
                direction="auto"
                responsive
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
                  {ShowModal === "Update" && (
                    <UpdateSubFee
                      selectIdClick={selectIdClick}
                      hideModal={hideModal}
                      getListSubFee={fetchData}
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

export default ApproveSubFee;
