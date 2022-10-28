import { useMemo, useState, useEffect, useCallback, useRef } from "react";
import { getData, postData } from "../Common/FuncAxios";
import DataTable from "react-data-table-component";
import moment from "moment";
import { Modal } from "bootstrap";
import DatePicker from "react-datepicker";
import UpdatePriceTable from "./UpdatePriceTable";

const ApprovePriceTable = (props) => {
  const { getDataApprove } = props;
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
      name: "Phân Loại Đối Tác",
      selector: (row) => row.maLoaiDoiTac,
    },
    {
      name: "Tên khách hàng",
      selector: (row) => row.tenKh,
    },
    {
      name: "Mã Hợp Đồng",
      selector: (row) => row.maHopDong,
      sortable: true,
    },
    {
      name: "Tên Hợp Đồng",
      selector: (row) => row.tenHopDong,
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
      name: "Tên Cung Đường",
      selector: (row) => row.tenCungDuong,
    },
    {
      name: "Phương Tiện Vận Tải",
      selector: (row) => row.maLoaiPhuongTien,
    },
    {
      name: "PTVC",
      selector: (row) => row.ptvc,
    },
    {
      name: "Loại Hàng Hóa",
      selector: (row) => row.maLoaiHangHoa,
    },
    {
      name: "Thời gian Hạn",
      selector: (row) => row.ngayHetHieuLuc,
      sortable: true,
    },
    {
      name: "Thời gian Tạo",
      selector: (row) => row.thoiGianTao,
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
    (async () => {
      var list = await getDataApprove();
      setData(list.data);
      setLoading(false);
    })();
  }, [props]);

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
        setSelectedRows([]);
        handleClearRows();
        fetchData(1);
      }
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
    await fetchData(page);
  };

  const handlePerRowsChange = async (newPerPage, page) => {
    setLoading(true);

    const dataCus = await getData(
      `PriceTable/GetListPriceTableApprove?PageNumber=${page}&PageSize=${newPerPage}&KeyWord=${keySearch}&fromDate=${fromDate}&toDate=${toDate}`
    );
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
                    className="btn btn-sm btn-default"
                    onClick={() => AcceptPriceTable(0)}
                  >
                    <i className="fas fa-thumbs-up"></i>
                  </button>

                  <button
                    type="button"
                    className="btn btn-sm btn-default mx-4"
                    onClick={() => AcceptPriceTable(1)}
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
                title="Duyệt Bảng Giá"
                columns={columns}
                data={data}
                progressPending={loading}
                pagination
                paginationServer
                selectableRows
                onSelectedRowsChange={handleChange}
                clearSelectedRows={toggledClearRows}
                paginationTotalRows={totalRows}
                onChangeRowsPerPage={handlePerRowsChange}
                onChangePage={handlePageChange}
                highlightOnHover
                responsive
                striped
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
    </>
  );
};

export default ApprovePriceTable;
