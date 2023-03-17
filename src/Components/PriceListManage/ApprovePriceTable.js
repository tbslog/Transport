import { useMemo, useState, useEffect, useCallback, useRef } from "react";
import { getData, postData } from "../Common/FuncAxios";
import DataTable from "react-data-table-component";
import moment from "moment";
import { Modal } from "bootstrap";
import DatePicker from "react-datepicker";
import UpdatePriceTable from "./UpdatePriceTable";
import ConfirmDialog from "../Common/Dialog/ConfirmDialog";
import { ToastError } from "../Common/FuncToast";

const ApprovePriceTable = (props) => {
  const { getDataApprove, checkShowModal, reLoadData } = props;
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
  const [toggledClearRows, setToggleClearRows] = useState(false);
  const [selectIdClick, setSelectIdClick] = useState({});

  const [isAccept, setIsAccept] = useState();
  const [ShowConfirm, setShowConfirm] = useState(false);

  const columns = useMemo(() => [
    {
      cell: (val) => (
        <button
          title="Cập nhật"
          onClick={() => handleEditButtonClick(val, SetShowModal("Edit"))}
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
      omit: true,
      name: "Id",
      selector: (row) => row.id,
    },
    {
      name: <div>Phân Loại Đối Tác</div>,
      selector: (row) => row.maLoaiDoiTac,
    },
    {
      name: <div>Tên khách hàng</div>,
      selector: (row) => <div className="text-wrap">{row.tenKh}</div>,
    },

    {
      name: <div>Mã Hợp Đồng</div>,
      selector: (row) => <div className="text-wrap">{row.maHopDong}</div>,
      sortable: true,
    },
    {
      name: <div>Tên Hợp Đồng</div>,
      selector: (row) => <div className="text-wrap">{row.tenHopDong}</div>,
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
      name: <div>Điểm Đóng Hàng</div>,
      selector: (row) => <div className="text-warp">{row.diemDau}</div>,
      sortable: true,
    },
    {
      name: <div>Điểm Trả Hàng</div>,
      selector: (row) => <div className="text-warp">{row.diemCuoi}</div>,
      sortable: true,
    },

    {
      name: <div>Điểm Lấy/Trả Rỗng</div>,
      selector: (row) => <div className="text-warp">{row.diemLayTraRong}</div>,
      sortable: true,
    },
    {
      name: <div>Phương Tiện Vận Tải</div>,
      selector: (row) => row.maLoaiPhuongTien,
    },
    {
      name: "PTVC",
      selector: (row) => row.ptvc,
    },
    {
      name: <div>Loại Hàng Hóa</div>,
      selector: (row) => row.maLoaiHangHoa,
    },
    {
      name: "Thời gian Tạo",
      selector: (row) => <div className="text-wrap">{row.thoiGianTao}</div>,
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

  useEffect(() => {
    setLoading(true);
    if (checkShowModal && Object.keys(checkShowModal).length > 0) {
      (async () => {
        fetchData(1);
        setLoading(false);
      })();
    }
  }, [props, checkShowModal]);

  const handleChange = (state) => {
    setSelectedRows(state.selectedRows);
  };

  const handleEditButtonClick = async (val) => {
    const getdata = await getData(`PriceTable/GetPriceTableById?id=${val.id}`);
    setSelectIdClick(getdata);
    showModalForm();
  };

  const AcceptPriceTable = async (isAccept) => {
    if (
      selectedRows &&
      selectedRows.length > 0 &&
      Object.keys(selectedRows).length > 0
    ) {
      let arr = [];
      selectedRows.map((val) => {
        arr.push({
          Id: val.id,
          IsAgree: isAccept,
        });
      });

      const SetApprove = await postData(`PriceTable/ApprovePriceTable`, {
        result: arr,
      });

      if (SetApprove === 1) {
        reLoadData();
        fetchData(1);
      }
      setSelectedRows([]);
      handleClearRows();
      setShowConfirm(false);
    }
  };

  const funcAgree = () => {
    if (selectedRows && selectedRows.length > 0) {
      AcceptPriceTable(isAccept);
    }
  };

  const ShowConfirmDialog = () => {
    if (selectedRows.length < 1) {
      ToastError("Vui lòng chọn bảng giá để duyệt");
      return;
    } else {
      setShowConfirm(true);
    }
  };

  const handleClearRows = () => {
    setToggleClearRows(!toggledClearRows);
  };

  const fetchData = async (page, KeyWord = "", fromDate = "", toDate = "") => {
    setLoading(true);

    if (KeyWord !== "") {
      KeyWord = keySearch;
    }
    fromDate = fromDate === "" ? "" : moment(fromDate).format("YYYY-MM-DD");
    toDate = toDate === "" ? "" : moment(toDate).format("YYYY-MM-DD");
    const dataCus = await getData(
      `PriceTable/GetListPriceTableApprove?PageNumber=${page}&PageSize=${perPage}&KeyWord=${KeyWord}&fromDate=${fromDate}&toDate=${toDate}`
    );
    setData(dataCus.data);
    setTotalRows(dataCus.totalRecords);
    setLoading(false);
  };

  const handlePageChange = async (page) => {
    await fetchData(page, keySearch, fromDate, toDate);
  };

  const handlePerRowsChange = async (newPerPage, page) => {
    setLoading(true);

    const dataCus = await getData(
      `PriceTable/GetListPriceTableApprove?PageNumber=${page}&PageSize=${newPerPage}&KeyWord=${keySearch}&fromDate=${fromDate}&toDate=${toDate}`
    );
    setData(dataCus.data);
    setPerPage(newPerPage);
    setTotalRows(dataCus.totalRecords);
    setLoading(false);
  };

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
      <section className="content">
        <div className="card">
          <div className="card-header">
            <div className="container-fruid">
              <div className="row">
                <div className="col col-sm">
                  <button
                    type="button"
                    className="btn btn-title btn-sm btn-default mx-1"
                    gloss="Duyệt Bảng Giá"
                    onClick={() => {
                      setIsAccept(0);
                      ShowConfirmDialog();
                    }}
                  >
                    <i className="fas fa-thumbs-up"></i>
                  </button>

                  <button
                    type="button"
                    className="btn btn-title btn-sm btn-default mx-4"
                    gloss="Không Duyệt Bảng Giá"
                    onClick={() => {
                      setIsAccept(1);
                      ShowConfirmDialog();
                    }}
                  >
                    <i className="fas fa-thumbs-down"></i>
                  </button>
                </div>
                <div className="col col-sm">
                  <div className="row">
                    <div className="col col-sm">
                      {/* <div className="input-group input-group-sm">
                        <select
                          className="form-control form-control-sm"
                          onChange={(e) =>
                            handleOnChangeVehicleType(e.target.value)
                          }
                          value={vehicleType}
                        >
                          <option value="">Phương tiện vận tải</option>
                          {listVehicleType &&
                            listVehicleType.map((val) => {
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
                      </div> */}
                    </div>
                    <div className="col col-sm">
                      {/* <div className="input-group input-group-sm">
                        <select
                          className="form-control form-control-sm"
                          onChange={(e) =>
                            handleOnChangeGoodsType(e.target.value)
                          }
                          value={goodsType}
                        >
                          <option value="">Loại Hàng Hóa</option>
                          {listGoodsType &&
                            listGoodsType.map((val) => {
                              return (
                                <option
                                  value={val.maLoaiHangHoa}
                                  key={val.maLoaiHangHoa}
                                >
                                  {val.tenLoaiHangHoa}
                                </option>
                              );
                            })}
                        </select>
                      </div> */}
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
                columns={columns}
                data={data}
                progressPending={loading}
                pagination
                paginationServer
                paginationRowsPerPageOptions={[10, 30, 50, 100]}
                selectableRows
                onSelectedRowsChange={handleChange}
                clearSelectedRows={toggledClearRows}
                paginationTotalRows={totalRows}
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
            <div className="row">
              {/* <div className="col-sm-3">
                <a
                  title="Tải Template Excel"
                  href={FileExcelImport}
                  download="Template Thêm mới Khách hàng.xlsx"
                  className="btn btn-sm btn-default mx-1"
                >
                  <i className="fas fa-download"></i>
                </a>
                <div className="upload-btn-wrapper">
                  <button
                    className="btn btn-sm btn-default mx-1"
                    title="Upload file Excel"
                  >
                    <i className="fas fa-upload"></i>
                  </button>
                  <input
                    type="file"
                    name="myfile"
                    onChange={(e) => handleExcelImportClick(e)}
                  />
                </div>
              </div> */}
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
                {ShowModal === "Edit" && (
                  <UpdatePriceTable
                    selectIdClick={selectIdClick}
                    hideModal={hideModal}
                    refeshData={fetchData}
                  />
                )}
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

export default ApprovePriceTable;
