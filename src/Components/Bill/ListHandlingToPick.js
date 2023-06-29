import { useMemo, useState, useEffect } from "react";
import { getData, postData } from "../Common/FuncAxios";
import DataTable from "react-data-table-component";
import DatePicker from "react-datepicker";
import moment from "moment";
import { ToastError } from "../Common/FuncToast";
import ConfirmDialog from "../Common/Dialog/ConfirmDialog";
import { Tooltip, OverlayTrigger } from "react-bootstrap";

const ListHandlingToPick = (props) => {
  const { cusId, datePay, reloadData, hideModal } = props;

  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(false);
  const [totalRows, setTotalRows] = useState(0);
  const [perPage, setPerPage] = useState(30);
  const [page, setPage] = useState(1);
  const [keySearch, setKeySearch] = useState("");

  const [listVehicleType, setListVehicleType] = useState([]);
  const [vehicleType, setVehicleType] = useState("");

  const [fromDate, setFromDate] = useState(new Date(moment().startOf("month")));
  const [toDate, setToDate] = useState(new Date());

  const [selectedRows, setSelectedRows] = useState([]);
  const [toggledClearRows, setToggleClearRows] = useState(false);
  const [ShowConfirm, setShowConfirm] = useState(false);

  useEffect(() => {
    (async () => {
      let getlistVehicleType = await getData(`Common/GetListVehicleType`);
      setListVehicleType(getlistVehicleType);
    })();
  }, []);

  useEffect(() => {
    if (cusId && datePay) {
      fetchData(1, "", fromDate, toDate, "");
    }
  }, [cusId, datePay]);

  const columns = useMemo(() => [
    {
      name: <div>ID</div>,
      selector: (row) => row.handlingId,
    },
    {
      name: <div>Đơn Vị Vận Tải</div>,
      selector: (row) => (
        <OverlayTrigger
          placement="top"
          overlay={
            <Tooltip id="tooltip">
              <strong>{row.tenNCC}</strong>
            </Tooltip>
          }
        >
          <div bsStyle="default">{row.tenNCC}</div>
        </OverlayTrigger>
      ),
    },
    {
      name: <div>Booking No</div>,
      selector: (row) => (
        <OverlayTrigger
          placement="top"
          overlay={
            <Tooltip id="tooltip">
              <strong>{row.maVanDonKH}</strong>
            </Tooltip>
          }
        >
          <div bsStyle="default">{row.maVanDonKH}</div>
        </OverlayTrigger>
      ),
    },
    // {
    //   name: <div>Hãng Tàu</div>,
    //   selector: (row) => <div className="text-wrap">{row.hangTau}</div>,
    // },
    {
      name: <div>Loại Vận Đơn</div>,
      selector: (row) => (
        <OverlayTrigger
          placement="top"
          overlay={
            <Tooltip id="tooltip">
              <strong>{row.loaiVanDon}</strong>
            </Tooltip>
          }
        >
          <div bsStyle="default">{row.loaiVanDon}</div>
        </OverlayTrigger>
      ),
    },
    {
      name: <div>Khách Hàng</div>,
      selector: (row) => (
        <OverlayTrigger
          placement="top"
          overlay={
            <Tooltip id="tooltip">
              <strong>{row.tenKH}</strong>
            </Tooltip>
          }
        >
          <div bsStyle="default">{row.tenKH}</div>
        </OverlayTrigger>
      ),
    },
    {
      name: <div>Account</div>,
      selector: (row) => (
        <OverlayTrigger
          placement="top"
          overlay={
            <Tooltip id="tooltip">
              <strong>{row.accountName}</strong>
            </Tooltip>
          }
        >
          <div bsStyle="default">{row.accountName}</div>
        </OverlayTrigger>
      ),
    },
    {
      name: <div>PTVC</div>,
      selector: (row) => (
        <OverlayTrigger
          placement="top"
          overlay={
            <Tooltip id="tooltip">
              <strong>{row.maPTVC}</strong>
            </Tooltip>
          }
        >
          <div bsStyle="default">{row.maPTVC}</div>
        </OverlayTrigger>
      ),
    },
    {
      name: <div>Điểm Lấy Hàng</div>,
      selector: (row) => (
        <OverlayTrigger
          placement="top"
          overlay={
            <Tooltip id="tooltip">
              <strong>{row.diemDau}</strong>
            </Tooltip>
          }
        >
          <div bsStyle="default">{row.diemDau}</div>
        </OverlayTrigger>
      ),
    },
    {
      name: <div>Điểm Hạ Hàng</div>,
      selector: (row) => (
        <OverlayTrigger
          placement="top"
          overlay={
            <Tooltip id="tooltip">
              <strong>{row.diemCuoi}</strong>
            </Tooltip>
          }
        >
          <div bsStyle="default">{row.diemCuoi}</div>
        </OverlayTrigger>
      ),
    },
    {
      name: <div>Điểm Lấy Rỗng</div>,
      selector: (row) => (
        <OverlayTrigger
          placement="top"
          overlay={
            <Tooltip id="tooltip">
              <strong>{row.diemLayRong}</strong>
            </Tooltip>
          }
        >
          <div bsStyle="default">{row.diemLayRong}</div>
        </OverlayTrigger>
      ),
    },
    {
      name: <div>Điểm Trả Rỗng</div>,
      selector: (row) => (
        <OverlayTrigger
          placement="top"
          overlay={
            <Tooltip id="tooltip">
              <strong>{row.diemTraRong}</strong>
            </Tooltip>
          }
        >
          <div bsStyle="default">{row.diemTraRong}</div>
        </OverlayTrigger>
      ),
    },
    {
      name: <div>Loại Phương Tiện</div>,
      selector: (row) => (
        <OverlayTrigger
          placement="top"
          overlay={
            <Tooltip id="tooltip">
              <strong>{row.loaiPhuongTien}</strong>
            </Tooltip>
          }
        >
          <div bsStyle="default">{row.loaiPhuongTien}</div>
        </OverlayTrigger>
      ),
    },
    {
      name: <div>Reuse CONT</div>,
      selector: (row) => row.reuse,
    },
    {
      name: <div>Ngày Tạo</div>,
      selector: (row) => moment(row.createdTime).format("YYYY-MM-DD HH:mm"),
    },
    // {
    //   name: <div>Đơn Giá KH</div>,
    //   selector: (row) => (
    //     <div className="text-wrap">
    //       {row.donGiaKH.toLocaleString("vi-VI", {
    //         style: "currency",
    //         currency: "VND",
    //       })}
    //     </div>
    //   ),
    // },
    // {
    //   name: <div>Đơn Giá NCC</div>,
    //   selector: (row) => (
    //     <div className="text-wrap">
    //       {row.donGiaNCC.toLocaleString("vi-VI", {
    //         style: "currency",
    //         currency: "VND",
    //       })}
    //     </div>
    //   ),
    // },
    // {
    //   name: <div>Phụ Phí HĐ</div>,
    //   selector: (row) => (
    //     <div className="text-wrap">
    //       {row.chiPhiHopDong.toLocaleString("vi-VI", {
    //         style: "currency",
    //         currency: "VND",
    //       })}
    //     </div>
    //   ),
    // },
    // {
    //   name: <div>Phụ Phí Phát Sinh</div>,
    //   selector: (row) => (
    //     <div className="text-wrap">
    //       {row.chiPhiPhatSinh.toLocaleString("vi-VI", {
    //         style: "currency",
    //         currency: "VND",
    //       })}
    //     </div>
    //   ),
    // },
  ]);

  const handleChange = (state) => {
    setSelectedRows(state.selectedRows);
  };

  const PickHandlingToBill = async () => {
    if (
      selectedRows &&
      selectedRows.length > 0 &&
      Object.keys(selectedRows).length > 0
    ) {
      let arr = [];
      selectedRows.map((val) => {
        arr.push(val.handlingId);
      });

      const add = await postData(`Bills/StoreDataHandlingToBill`, {
        cusId: cusId.value,
        ids: arr,
        dateBlock: datePay,
      });

      if (add === 1) {
        await reloadData();
      }

      setSelectedRows([]);
      handleClearRows();
      setShowConfirm(false);
      hideModal();
    }
  };

  const funcAgree = () => {
    if (selectedRows && selectedRows.length > 0) {
      PickHandlingToBill();
    }
  };

  const ShowConfirmDialog = () => {
    if (selectedRows.length < 1) {
      ToastError("Vui lòng chọn chuyến");
      return;
    } else {
      setShowConfirm(true);
    }
  };

  const handleClearRows = () => {
    setToggleClearRows(!toggledClearRows);
  };

  const fetchData = async (
    page,
    KeyWord = "",
    fromDate = "",
    toDate = "",
    vehicleType = ""
  ) => {
    if (KeyWord !== "") {
      KeyWord = keySearch;
    }
    setLoading(true);

    fromDate = fromDate === "" ? "" : moment(fromDate).format("YYYY-MM-DD");
    toDate = toDate === "" ? "" : moment(toDate).format("YYYY-MM-DD");

    const dataCus = await getData(
      `Bills/GetListHandlingToPick?PageNumber=${page}&PageSize=${perPage}&KeyWord=${KeyWord}&fromDate=${fromDate}&toDate=${toDate}&customerId=${cusId.value}&vehicleType=${vehicleType}`
    );

    setLoading(false);
    setData(dataCus.data);
    setTotalRows(dataCus.totalRecords);
  };

  const handlePageChange = async (page) => {
    await fetchData(page, keySearch, fromDate, toDate);
    setPage(page);
  };

  const handlePerRowsChange = async (newPerPage, page) => {
    setLoading(true);
    console.log(newPerPage);
    const dataCus = await getData(
      `Bills/GetListHandlingToPick?PageNumber=${page}&PageSize=${newPerPage}&KeyWord=${keySearch}&fromDate=${moment(
        fromDate
      ).format("YYYY-MM-DD")}&toDate=${moment(toDate).format(
        "YYYY-MM-DD"
      )}&customerId=${cusId.value}&vehicleType=${vehicleType}`
    );
    setData(dataCus.data);
    setPerPage(newPerPage);
    setTotalRows(dataCus.totalRecords);
    setLoading(false);
  };

  const handleSearchClick = async () => {
    await fetchData(1, keySearch, fromDate, toDate, vehicleType);
  };

  const handleRefeshDataClick = async () => {
    await fetchData(1, "", fromDate, toDate, vehicleType);
  };

  const handleOnChangeVehicleType = async (val) => {
    await fetchData(1, keySearch, fromDate, toDate, val);
    setVehicleType(val);
  };

  return (
    <>
      <section className="content" style={{ height: "80vh" }}>
        <div className="card">
          <div className="card-header">
            <div className="container-fruid">
              <div className="row">
                <div className="col col-sm">
                  <button
                    type="button"
                    className="btn btn-title btn-sm btn-default mx-1"
                    gloss="Thêm Chuyến"
                    onClick={() => {
                      ShowConfirmDialog();
                    }}
                  >
                    <i className="fas fa-plus"></i>
                  </button>
                </div>
                <div className="col col-sm">
                  <div className="row">
                    {/* <div className="col col-sm">
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
                      </div>
                    </div> */}
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
                paginationRowsPerPageOptions={[30, 50, 80, 100]}
                onSelectedRowsChange={handleChange}
                paginationTotalRows={totalRows}
                onChangeRowsPerPage={handlePerRowsChange}
                onChangePage={handlePageChange}
                highlightOnHover
                selectableRows
                clearSelectedRows={toggledClearRows}
                striped
                direction="auto"
                responsive
                fixedHeader
                dense
                fixedHeaderScrollHeight="60vh"
              />
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

export default ListHandlingToPick;
