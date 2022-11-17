import { useMemo, useState, useEffect, useCallback, useRef } from "react";
import { getData, postFile, getDataCustom } from "../Common/FuncAxios";
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
  const [keySearch, setKeySearch] = useState("");

  const [ShowModal, SetShowModal] = useState("");
  const [modal, setModal] = useState(null);
  const parseExceptionModal = useRef();

  const [selectedRows, setSelectedRows] = useState([]);
  const [selectIdClick, setSelectIdClick] = useState({});

  const [fromDate, setFromDate] = useState("");
  const [toDate, setToDate] = useState("");
  const [customerId, setCustomerId] = useState("");
  const [listky, setListKy] = useState([]);
  const [ky, setKy] = useState(1);

  const columns = useMemo(() => [
    {
      name: "Xem Chi Tiết",
      cell: (val) => (
        <button
          onClick={() =>
            handleButtonClick(val, SetShowModal("DetailBillByTransport"))
          }
          type="button"
          className="btn btn-sm btn-default"
        >
          <i className="fas fa-file-invoice-dollar"></i>
        </button>
      ),
      ignoreRowClick: true,
      allowOverflow: true,
      button: true,
    },
    {
      name: "Mã Vận Đơn",
      selector: (row) => row.maVanDon,
    },
    {
      name: "Loại Vận Đơn",
      selector: (row) => row.loaiVanDon,
    },
    {
      name: "Mã Khách Hàng",
      selector: (row) => row.maKh,
    },
    {
      name: "Tên Khách Hàng",
      selector: (row) => row.tenKh,
    },
    {
      name: "Mã Cung Đường",
      selector: (row) => row.maCungDuong,
    },
    {
      name: "Điểm Lấy Hàng",
      selector: (row) => row.diemLayHang,
    },
    {
      name: "Điểm Trả Hàng",
      selector: (row) => row.diemTraHang,
    },
    {
      name: "Tổng Khối Lượng",
      selector: (row) => row.tongKhoiLuong,
    },
    {
      name: "Tổng Thể Tích",
      selector: (row) => row.tongTheTich,
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

  const fetchData = async (page, KeyWord = "", customerId, ky) => {
    if (!customerId || !ky) {
      ToastError("Vui lòng chọn kỳ thanh toán và mã khách hàng");
      return;
    }

    setLoading(true);
    const dataBills = await getData(
      `Bills/GetListTransportByCustomerId?customerId=${customerId}&ky=${ky}&PageNumber=${page}&PageSize=${perPage}&KeyWord=${KeyWord}`
    );

    setData(dataBills.data);
    setTotalRows(dataBills.totalRecords);
    setLoading(false);
  };

  const handlePageChange = async (page) => {
    await fetchData(page, "", fromDate, toDate, customerId, ky);
  };

  const handlePerRowsChange = async (newPerPage, page) => {
    setLoading(true);

    const dataBills = await getData(
      `Bills/GetListTransportByCustomerId?customerId=${customerId}&ky=${ky}&PageNumber=${page}&PageSize=${newPerPage}&KeyWord=${keySearch}`
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

  const handleSearchByContractId = async () => {
    if (customerId) {
      const getListKy = await getData(
        `Bills/GetListKy?customerId=${customerId}`
      );
      setListKy(getListKy);
    }
  };

  const handleSearchClick = () => {
    fetchData(1, keySearch, customerId, ky);
  };

  const handleRefeshDataClick = () => {
    setKeySearch("");
    setFromDate("");
    setToDate("");
    setData([]);
  };

  return (
    <>
      <section className="content-header">
        <div className="container-fluid">
          <div className="row mb-2">
            <div className="col-sm-6">
              <h1>Danh sách hóa đơn</h1>
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
                <div className="col-sm-3"></div>
                <div className="col-sm-3 ">
                  <div className="input-group input-group-sm">
                    <input
                      placeholder="Nhập Mã Khách Hàng"
                      type="text"
                      className="form-control"
                      value={customerId}
                      onChange={(e) => setCustomerId(e.target.value)}
                    />
                    <span className="input-group-append">
                      <button
                        type="button"
                        className="btn btn-sm btn-default"
                        onClick={() => handleSearchByContractId()}
                      >
                        <i className="fas fa-arrow-circle-right"></i>
                      </button>
                    </span>
                  </div>
                </div>
              </div>
            </div>
          </div>

          {listky && listky.length > 0 && (
            <div className="card-header">
              <div className="container-fruid">
                <div className="row">
                  <div className="col-sm-3">
                    <div className="col-sm-3">
                      <button
                        type="button"
                        className="btn btn-sm btn-default mx-1"
                        onClick={() =>
                          showModalForm(SetShowModal("DetailBill"))
                        }
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
          )}

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
                  {ShowModal === "DetailBill" && (
                    <DetailBill customerId={customerId} ky={ky} />
                  )}
                  {ShowModal === "DetailBillByTransport" && (
                    <DetailBillByTransport dataClick={selectIdClick} ky={ky} />
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
