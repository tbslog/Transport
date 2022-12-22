import { useMemo, useState, useEffect, useCallback, useRef } from "react";
import { getData, postFile, getDataCustom, getFile } from "../Common/FuncAxios";
import DataTable from "react-data-table-component";
import moment from "moment";
import { Modal } from "bootstrap";
import DatePicker from "react-datepicker";
import { ToastError } from "../Common/FuncToast";
import DetailBill from "./DetailBill";
import DetailBillByTransport from "./DetailBillByContract";

const BillPage = () => {
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(false);
  const [totalRows, setTotalRows] = useState(0);
  const [perPage, setPerPage] = useState(10);
  const [page, setPage] = useState(0);
  const [keySearch, setKeySearch] = useState("");

  const [ShowModal, SetShowModal] = useState("");
  const [modal, setModal] = useState(null);
  const parseExceptionModal = useRef();

  const [selectedRows, setSelectedRows] = useState([]);
  const [selectIdClick, setSelectIdClick] = useState({});

  const [fromDate, setFromDate] = useState("");
  const [toDate, setToDate] = useState("");

  const columns = useMemo(() => [
    {
      name: <div>Hóa Đơn Vận Đơn</div>,
      cell: (val) => (
        <button
          onClick={() =>
            handleButtonClick(val, SetShowModal("DetailBillByTransport"))
          }
          type="button"
          className="btn btn-title btn-sm btn-default mx-1"
          gloss="Xem Hóa Đơn "
        >
          <i className="fas fa-file-invoice-dollar"></i>
        </button>
      ),
      ignoreRowClick: true,
      allowOverflow: true,
      button: true,
    },
    {
      name: <div>Mã Chuyến</div>,
      selector: (row) => row.maChuyen,
    },
    {
      selector: (row) => row.maVanDon,
      omit: true,
    },
    {
      name: <div>Mã Vận Đơn</div>,
      selector: (row) => <div className="text-wrap">{row.maVanDonKH}</div>,
    },
    {
      name: <div>Loại Vận Đơn</div>,
      selector: (row) => <div className="text-wrap">{row.loaiVanDon}</div>,
    },
    {
      name: <div>Loại Hàng Hóa</div>,
      selector: (row) => <div className="text-wrap">{row.loaiHangHoa}</div>,
    },
    {
      name: <div>Loại Phương Tiện</div>,
      selector: (row) => <div className="text-wrap">{row.loaiPhuongTien}</div>,
    },
    {
      name: <div>PTVC</div>,
      selector: (row) => <div className="text-wrap">{row.maPTVC}</div>,
    },
    {
      name: <div>Mã Khách Hàng</div>,
      selector: (row) => <div className="text-wrap">{row.maKh}</div>,
      omit: true,
    },
    {
      name: <div>Khách Hàng</div>,
      selector: (row) => <div className="text-wrap">{row.khachHang}</div>,
    },
    {
      name: <div>Đơn Vị Vận Tải</div>,
      selector: (row) => <div className="text-wrap">{row.donViVanTai}</div>,
    },
    {
      name: <div>Đơn Giá KH</div>,
      selector: (row) => (
        <div className="text-wrap">
          {row.donGiaKH.toLocaleString("vi-VI", {
            style: "currency",
            currency: "VND",
          })}
        </div>
      ),
    },
    {
      name: <div>Đơn Giá NCC</div>,
      selector: (row) => (
        <div className="text-wrap">
          {row.donGiaNCC.toLocaleString("vi-VI", {
            style: "currency",
            currency: "VND",
          })}
        </div>
      ),
    },
    {
      name: <div>Doanh Thu</div>,
      selector: (row) => (
        <div className="text-wrap">
          {row.doanhThu.toLocaleString("vi-VI", {
            style: "currency",
            currency: "VND",
          })}
        </div>
      ),
    },
    {
      name: <div>Lợi Nhuận</div>,
      selector: (row) => (
        <div className="text-wrap">
          {row.loiNhuan.toLocaleString("vi-VI", {
            style: "currency",
            currency: "VND",
          })}
        </div>
      ),
    },
    {
      name: <div>Phụ Phí HĐ</div>,
      selector: (row) => (
        <div className="text-wrap">
          {row.chiPhiHopDong.toLocaleString("vi-VI", {
            style: "currency",
            currency: "VND",
          })}
        </div>
      ),
    },
    {
      name: <div>Phụ Phí Phát Sinh</div>,
      selector: (row) => (
        <div className="text-wrap">
          {row.chiPhiPhatSinh.toLocaleString("vi-VI", {
            style: "currency",
            currency: "VND",
          })}
        </div>
      ),
    },
  ]);

  useEffect(() => {
    fetchData(1);
  }, []);

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

  const fetchData = async (page, KeyWord = "", fromDate, toDate) => {
    setLoading(true);
    if (KeyWord !== "") {
      KeyWord = keySearch;
    }
    fromDate = !fromDate ? "" : moment(fromDate).format("YYYY-MM-DD");
    toDate = !toDate ? "" : moment(toDate).format("YYYY-MM-DD");

    const dataBills = await getData(
      `Bills/GetListBillHandling?PageNumber=${page}&PageSize=${perPage}&KeyWord=${KeyWord}&fromDate=${fromDate}&toDate=${toDate}`
    );

    setData(dataBills.data);
    setTotalRows(dataBills.totalRecords);
    setLoading(false);
  };

  const handlePageChange = (page) => {
    setPage(page);
    fetchData(page, keySearch, fromDate, toDate);
  };

  const handlePerRowsChange = async (newPerPage, page) => {
    setLoading(true);

    const dataBills = await getData(
      `Bills/GetListBillHandling?PageNumber=${page}&PageSize=${newPerPage}&KeyWord=${keySearch}&fromDate=${fromDate}&toDate=${toDate}`
    );
    setData(dataBills);
    setPerPage(newPerPage);
    setLoading(false);
  };

  const handleChange = useCallback((state) => {
    setSelectedRows(state.selectedRows);
  }, []);

  const handleButtonClick = async (value) => {
    setSelectIdClick(value);
    showModalForm();
  };

  // const handleSearchByContractId = async () => {
  //   if (customerId) {
  //     const getListKy = await getData(
  //       `Bills/GetListKy?customerId=${customerId}`
  //     );
  //     setListKy(getListKy);
  //   }
  // };

  const handleSearchClick = () => {
    fetchData(page, keySearch, fromDate, toDate);
  };

  const handleRefeshDataClick = () => {
    setKeySearch("");
    setFromDate("");
    setToDate("");
    setData([]);
  };

  const handleExportExcel = async () => {
    if (!fromDate || !toDate) {
      ToastError("Vui lòng chọn mốc thời gian");
      return;
    }

    setLoading(true);

    let startDate = moment(fromDate).format("YYYY-MM-DD");
    let endDate = moment(toDate).format("YYYY-MM-DD");
    const getFileDownLoad = await getFile(
      `Bills/ExportExcelBill?KeyWord=${keySearch}&fromDate=${startDate}&toDate=${endDate}`,
      "HoaDon" + moment(new Date()).format("DD/MM/YYYY HHmmss")
    );
    setLoading(false);
  };

  return (
    <>
      <section className="content-header">
        <div className="container-fluid">
          <div className="row mb-2">
            <div className="col-sm-6">
              <h1>Danh sách Chuyến Đã Hoàn Thành</h1>
            </div>
          </div>
        </div>
      </section>

      <section className="content">
        <div className="card">
          <div className="card-header">
            <div className="container-fruid">
              <div className="row">
                <div className="col-sm-3"></div>
                <div className="col-sm-3"></div>
                <div className="col-sm-3">
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
                <div className="col-sm-3 ">
                  <div className="input-group input-group-sm">
                    <input
                      placeholder="Tìm Kiếm"
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

          {/* <div className="card-header">
            <div className="container-fruid">
              <div className="row">
                <div className="col-sm-3">
                  <div className="col-sm-3">
                    <button
                      type="button"
                      className="btn btn-title btn-sm btn-default mx-1"
                      gloss="Xem Hóa Đơn Kỳ"
                      onClick={() => showModalForm(SetShowModal("DetailBill"))}
                    >
                      <i className="fas fa-money-bill-alt"></i>
                    </button>
                  </div>
                </div>
                <div className="col-sm-3">
                  <div className="row">
                    <div className="col col-sm"></div>
                    <div className="col col-sm"></div>
                  </div>
                </div>
                <div className="col-sm-3">
                  <div className="row">
                    <div className="col col-sm">
                      <div className="input-group input-group-sm">
                        <select
                          className="form-control form-control-sm"
                          onChange={(e) => setKy(e.target.value)}
                          value={ky}
                        >
                          {listky.map((val) => {
                            return (
                              <option value={val.ky} key={val.ky}>
                                {"Kỳ Thanh Toán: " + val.ky}
                              </option>
                            );
                          })}
                        </select>
                      </div>
                    </div>
                  </div>
                </div>
                <div className="col-sm-3 "></div>
              </div>
            </div>
          </div> */}

          <div className="card-body">
            <div className="container-datatable" style={{ height: "50vm" }}>
              <DataTable
                columns={columns}
                data={data}
                progressPending={loading}
                pagination
                paginationServer
                paginationTotalRows={totalRows}
                onSelectedRowsChange={handleChange}
                onChangeRowsPerPage={handlePerRowsChange}
                onChangePage={handlePageChange}
                highlightOnHover
                striped
                direction="auto"
                responsive
              />
            </div>
          </div>
          <div className="card-footer">
            <div className="row">
              <div className="col-sm-3">
                <button
                  // href={FileExcelImport}
                  onClick={() => handleExportExcel()}
                  className="btn btn-title btn-sm btn-default mx-1"
                  gloss="Tải File Excel"
                  type="button"
                >
                  <i className="fas fa-file-excel"></i>
                </button>
                {/* <div className="upload-btn-wrapper">
                  <button className="btn btn-sm btn-default mx-1">
                    <i className="fas fa-upload"></i>
                  </button>
                  <input
                    type="file"
                    name="myfile"
                    // onChange={(e) => handleExcelImportClick(e)}
                  />
                </div> */}
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
                  {/* {ShowModal === "DetailBill" && (
                    <DetailBill customerId={customerId} ky={ky} />
                  )} */}
                  {ShowModal === "DetailBillByTransport" && (
                    <DetailBillByTransport dataClick={selectIdClick} />
                  )}
                </>
              </div>
            </div>
          </div>
        </div>
      </section>
    </>
  );
};

export default BillPage;
