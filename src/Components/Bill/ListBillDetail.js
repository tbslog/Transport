import { useState, useEffect, useRef } from "react";
import { getData, getFile, postData } from "../Common/FuncAxios";
import { Modal } from "bootstrap";
import Logo from "../../Image/Logo/logo2x.png";
import "./bill.css";
import { ToastError } from "../Common/FuncToast";
import DatePicker from "react-datepicker";
import moment from "moment";
import { Tooltip, OverlayTrigger } from "react-bootstrap";
import ListHandlingToPick from "./ListHandlingToPick";
import ConfirmDialog from "../Common/Dialog/ConfirmDialog";

const ListBillDetail = (props) => {
  const { customer, datePay, listBank, cusType } = props;
  const [isLoading, setIsLoading] = useState(true);
  const [dataBill, setDataBill] = useState([]);
  const [totalSum, setTotalSum] = useState({});

  const [datetime, setDatetime] = useState(new Date());
  const [bankSelected, setBankSelected] = useState("VCB");

  const [ShowModal, SetShowModal] = useState("");
  const [modal, setModal] = useState(null);
  const parseExceptionModal = useRef();
  const [titleModal, setTitleModal] = useState("");

  const [ShowConfirm, setShowConfirm] = useState(false);

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
    if (customer && datePay) {
      getDataBill(customer.value, datePay);
    } else {
      ToastError("Vui lòng chọn Khách Hàng, và kỳ thanh toán để xem");
      return;
    }
  }, [props, datePay, customer]);

  const getDataBill = async (customerId, datePay) => {
    setIsLoading(true);
    if (customerId && datePay) {
      datePay = moment(datePay).format("YYYY-MM-DD");

      var dataBill = await getData(
        `Bills/GetBillByCustomerId?customerId=${customerId}&datePay=${datePay}&dateTime=${moment(
          datetime
        ).format("YYYY-MM-DD")}&bank=${bankSelected}`
      );

      if (dataBill.billReuslt && dataBill.billReuslt.length > 0) {
        setDataBill(dataBill.billReuslt);
        var sumData = getTotalPrice(dataBill.billReuslt);
        if (sumData) {
          setTotalSum(sumData);
        } else {
          setTotalSum({});
        }
      }
    }
    setIsLoading(false);
  };

  const getTotalPrice = (billReuslt) => {
    let total = 0;
    let totalsf = 0;
    if (billReuslt && billReuslt.length > 0) {
      billReuslt.forEach((br) => {
        if (br.listHandling && br.listHandling.length > 0) {
          br.listHandling.forEach((hl) => {
            total =
              total + (cusType === "KH" ? hl.giaQuyDoiKh : hl.giaQuyDoiNcc);

            hl.listSubFeeByContract.forEach((val) => {
              totalsf = totalsf + val.priceTransfer;
            });
            hl.listSubFeeIncurreds.forEach((val) => {
              totalsf = totalsf + val.price;
            });
          });
        }
      });
    }

    return { total, totalsf };
  };

  const funcAgree = async () => {
    await blockDataBill();
  };

  const blockDataBill = async () => {
    if (customer && datePay) {
      let block = await postData(
        `Bills/BlockDataBillBy?customerId=${customer.value}&datePay=${moment(
          datePay
        ).format("YYYY-MM-DD")}&dateTime=${moment(datetime).format(
          "YYYY-MM-DD"
        )}&bank=${bankSelected}`
      );
      setShowConfirm(false);
    }
  };

  const handleOnChangeExchangeRate = async () => {
    await getDataBill(customer.value, datePay);
  };

  const handleShowModalClick = () => {
    showModalForm();
  };

  const handleExportExcel = async () => {
    setIsLoading(true);
    let custype = "";

    if (cusType === "KH") {
      custype = "customerId";
    }

    if (cusType === "NCC") {
      custype = "supplierId";
    }

    const getFileDownLoad = await getFile(
      `Bills/ExportExcelBill?${custype}=${customer.value}&date=${moment(
        datePay
      ).format("YYYY-MM-DD")}&customerType=`,
      "BILL" + moment(new Date()).format("DD/MM/YYYY HHmmss")
    );
    setIsLoading(false);
  };

  return (
    <>
      {isLoading && isLoading === true ? (
        <div>Loading...</div>
      ) : (
        <div>
          <div className="page-content">
            <div className="page-header text-blue-d2">
              {/* <h1 className="page-title text-secondary-d1">
               <img src={Logo}></img>
             </h1> */}

              <div className="page-tools">
                <div className="row">
                  <div className="col col-sm">
                    <div className="form-group">
                      <label>Ngày Quy Đổi Tỉ Giá</label>
                      <DatePicker
                        selected={datetime}
                        onChange={(date) => {
                          setDatetime(date);
                        }}
                        dateFormat="dd/MM/yyyy"
                        className="form-control form-control-sm"
                        placeholderText="Ngày Quy Đổi Tỉ Giá"
                        value={datetime}
                      />
                    </div>
                  </div>
                  <div className="col col-sm">
                    <div className="form-group">
                      <label>Ngân Hàng Quy Đổi</label>
                      <select
                        className="form-control form-control-sm"
                        onChange={(e) => {
                          setBankSelected(e.target.value);
                        }}
                        value={bankSelected}
                      >
                        {listBank &&
                          listBank.length > 0 &&
                          listBank.map((val) => {
                            return (
                              <option
                                value={val.maNganHang}
                                key={val.maNganHang}
                              >
                                {val.tenNganHang}
                              </option>
                            );
                          })}
                      </select>
                    </div>
                  </div>
                  <div className="col col-sm">
                    <div className="form-group">
                      <label>
                        <br></br>
                      </label>
                      <OverlayTrigger
                        placement="top"
                        overlay={
                          <Tooltip id="tooltip">
                            <strong>Thay Đổi Tỉ Giá</strong>
                          </Tooltip>
                        }
                      >
                        <div bsStyle="default">
                          <div className="form-group">
                            <button
                              onClick={() => handleOnChangeExchangeRate()}
                              className="btn btn-sm btn-primary"
                            >
                              <i className="fas fa-exchange-alt"></i>
                            </button>
                          </div>
                        </div>
                      </OverlayTrigger>
                    </div>
                  </div>
                </div>
              </div>
              <div className="page-tools">
                <div className="row">
                  <div className="col col-sm">
                    <div className="form-group">
                      <OverlayTrigger
                        placement="top"
                        overlay={
                          <Tooltip id="tooltip">
                            <strong>Xuất File Excel</strong>
                          </Tooltip>
                        }
                      >
                        <div bsStyle="default">
                          <div className="form-group">
                            <button
                              onClick={() => handleExportExcel()}
                              className="btn btn-sm btn-success"
                            >
                              <i className="fas fa-file-excel"></i>
                            </button>
                          </div>
                        </div>
                      </OverlayTrigger>
                    </div>
                  </div>
                  <div className="col col-sm">
                    <div className="form-group">
                      <OverlayTrigger
                        placement="top"
                        overlay={
                          <Tooltip id="tooltip">
                            <strong>Thêm Chuyến</strong>
                          </Tooltip>
                        }
                      >
                        <div bsStyle="default">
                          <div className="form-group">
                            <button
                              onClick={() => {
                                handleShowModalClick();
                                SetShowModal("ShowListHandlingPick");
                                setTitleModal("Thêm chuyến vào kỳ");
                              }}
                              className="btn btn-sm btn-warning"
                            >
                              <i className="fas fa-plus"></i>
                            </button>
                          </div>
                        </div>
                      </OverlayTrigger>
                    </div>
                  </div>
                  <div className="col col-sm">
                    <div className="form-group">
                      <OverlayTrigger
                        placement="top"
                        overlay={
                          <Tooltip id="tooltip">
                            <strong>Chốt Sản Lượng Kỳ</strong>
                          </Tooltip>
                        }
                      >
                        <div bsStyle="default">
                          <div className="form-group">
                            <button
                              className="btn btn-sm btn-danger"
                              onClick={() => setShowConfirm(true)}
                            >
                              <i className="fas fa-money-bill"></i>
                            </button>
                          </div>
                        </div>
                      </OverlayTrigger>
                    </div>
                  </div>
                </div>
              </div>
            </div>
            <div className="col-12">
              <div className="row mt-4">
                <div className="col-12 col-lg-12">
                  <div className="row">
                    <div className="col-12">
                      <div className="text-center text-150">
                        <h1 className="text-default-d3">{customer.label}</h1>
                      </div>
                    </div>
                  </div>
                  {/* .row */}
                  <hr className="row brc-default-l1 mx-n1 mb-4" />
                  <div className="row">
                    {/* <div className="col-sm-6">
                     <div>
                       <span className="text-sm text-grey-m2 align-middle">
                         To:
                       </span>
                       <span className="text-600 text-110 text-blue align-middle">
                         Alex Doe
                       </span>
                     </div>
                     <div className="text-grey-m2">
                       <div className="my-1">Street, City</div>
                       <div className="my-1">State, Country</div>
                       <div className="my-1">
                         <i className="fa fa-phone fa-flip-horizontal text-secondary" />
                         <b className="text-600">111-111-111</b>
                       </div>
                     </div>
                   </div>
                   <div className="text-95 col-sm-6 align-self-start d-sm-flex justify-content-end">
                     <hr className="d-sm-none" />
                     <div className="text-grey-m2">
                       <div className="mt-1 mb-2 text-secondary-m1 text-600 text-125">
                         Invoice
                       </div>
                       <div className="my-2">
                         <i className="fa fa-circle text-blue-m2 text-xs mr-1" />
                         <span className="text-600 text-90">ID:</span> #111-222
                       </div>
                       <div className="my-2">
                         <i className="fa fa-circle text-blue-m2 text-xs mr-1" />
                         <span className="text-600 text-90">Issue Date:</span>
                         Oct 12, 2019
                       </div>
                       <div className="my-2">
                         <i className="fa fa-circle text-blue-m2 text-xs mr-1" />
                         <span className="text-600 text-90">Status:</span>
                         <span className="badge badge-warning badge-pill px-25">
                           Unpaid
                         </span>
                       </div>
                     </div>
                   </div> */}
                  </div>
                  <div className="mt-4">
                    <table className="table  table-sm table-responsive-sm">
                      {dataBill &&
                        dataBill.length > 0 &&
                        dataBill.map((val, index) => {
                          return (
                            <>
                              <table
                                className="table  table-bordered table-sm table-responsive-sm"
                                style={{ border: "3px solid" }}
                              >
                                <thead>
                                  <tr>
                                    <th scope="col">{index + 1}</th>
                                    <th scope="col">Booking No</th>
                                    <th scope="col">Account</th>
                                    <th scope="col">Loại Vận Đơn</th>
                                    <th scope="col">Điểm Đóng Hàng</th>
                                    <th scope="col">Điểm Hạ Hàng</th>
                                  </tr>
                                </thead>
                                <tbody>
                                  <tr>
                                    <th scope="row"></th>
                                    <td>{val.maVanDonKH}</td>
                                    <td>{val.accountName}</td>
                                    <td>{val.loaiVanDon}</td>
                                    <td>{val.diemLayHang}</td>
                                    <td>{val.diemTraHang}</td>
                                  </tr>

                                  <tr>
                                    <td colSpan={2}></td>
                                    <td colSpan={4}>
                                      <table className="table table-bordered table-sm table-responsive-sm">
                                        {val.listHandling &&
                                          val.listHandling.length > 0 &&
                                          val.listHandling.map(
                                            (val1, index1) => {
                                              return (
                                                <>
                                                  <thead>
                                                    <tr>
                                                      <th>Chuyến</th>
                                                      <th>Điểm Lấy Rỗng</th>
                                                      <th>Điểm Trả Rỗng</th>
                                                      <th>Đơn Vị Tính</th>
                                                      {/* <th>Đơn Vị Vận Tải</th> */}
                                                      <th>Loại Hàng Hóa</th>
                                                      <th>Loại Phương Tiện</th>
                                                      <th>Đơn Giá Tuyến</th>
                                                      <th>Loại Tiền Tệ</th>
                                                      <th>Giá Trị Quy Đổi</th>
                                                    </tr>
                                                  </thead>
                                                  <tbody>
                                                    <tr>
                                                      <th>{index1 + 1}</th>
                                                      <td>
                                                        {val1.diemLayRong}
                                                      </td>
                                                      <td>
                                                        {val1.diemTraRong}
                                                      </td>
                                                      <td>{val1.donViTinh}</td>
                                                      {/* <td>{val1.donViVanTai}</td> */}
                                                      <td>
                                                        {val1.loaiHangHoa}
                                                      </td>
                                                      <td>
                                                        {val1.loaiPhuongTien}
                                                      </td>
                                                      <td>
                                                        {cusType === "KH"
                                                          ? val1.donGiaKh
                                                          : val1.donGiaNcc}
                                                      </td>
                                                      <td>
                                                        {cusType === "KH"
                                                          ? val1.loaiTienTeKh
                                                          : val1.loaiTienTeNcc}
                                                      </td>
                                                      <td>
                                                        {(cusType === "KH"
                                                          ? val1.giaQuyDoiKh
                                                          : val1.giaQuyDoiNcc
                                                        ).toLocaleString(
                                                          "vi-VI",
                                                          {
                                                            style: "currency",
                                                            currency: "VND",
                                                          }
                                                        )}
                                                      </td>
                                                    </tr>

                                                    {val1.listSubFeeByContract &&
                                                      val1.listSubFeeByContract
                                                        .length > 0 && (
                                                        <>
                                                          <tr>
                                                            <td
                                                              colSpan={2}
                                                            ></td>
                                                            <td colSpan={7}>
                                                              <table className="table table-bordered table-sm table-responsive-sm">
                                                                <thead>
                                                                  <tr>
                                                                    <th>
                                                                      # Phụ Phí
                                                                      Theo Hợp
                                                                      Đồng #
                                                                    </th>
                                                                    <th>
                                                                      Tên Phụ
                                                                      Phí
                                                                    </th>
                                                                    <th>
                                                                      Đơn Giá
                                                                    </th>
                                                                    <th>
                                                                      Loại Tiền
                                                                      Tệ
                                                                    </th>
                                                                    <th>
                                                                      Giá Trị
                                                                      Quy Đổi
                                                                    </th>
                                                                  </tr>
                                                                </thead>
                                                                <tbody>
                                                                  {val1.listSubFeeByContract.map(
                                                                    (
                                                                      val2,
                                                                      index2
                                                                    ) => {
                                                                      return (
                                                                        <>
                                                                          <tr>
                                                                            <td>
                                                                              {index2 +
                                                                                1}
                                                                            </td>
                                                                            <td>
                                                                              {
                                                                                val2.sfName
                                                                              }
                                                                            </td>
                                                                            <td>
                                                                              {
                                                                                val2.unitPrice
                                                                              }
                                                                            </td>
                                                                            <td>
                                                                              {
                                                                                val2.priceType
                                                                              }
                                                                            </td>
                                                                            <td>
                                                                              {val2.priceTransfer.toLocaleString(
                                                                                "vi-VI",
                                                                                {
                                                                                  style:
                                                                                    "currency",
                                                                                  currency:
                                                                                    "VND",
                                                                                }
                                                                              )}
                                                                            </td>
                                                                          </tr>
                                                                        </>
                                                                      );
                                                                    }
                                                                  )}
                                                                </tbody>
                                                              </table>
                                                            </td>
                                                          </tr>
                                                        </>
                                                      )}

                                                    {val1.listSubFeeIncurreds &&
                                                      val1.listSubFeeIncurreds
                                                        .length > 0 && (
                                                        <>
                                                          <tr>
                                                            <td
                                                              colSpan={2}
                                                            ></td>
                                                            <td colSpan={7}>
                                                              <table className="table table-bordered table-sm table-responsive-sm">
                                                                <thead>
                                                                  <tr>
                                                                    <th>
                                                                      # Phụ Phí
                                                                      Phát Sinh
                                                                      #
                                                                    </th>
                                                                    <th>
                                                                      Tên Phụ
                                                                      Phí
                                                                    </th>
                                                                    <th>
                                                                      Đơn Giá
                                                                    </th>
                                                                  </tr>
                                                                </thead>
                                                                <tbody>
                                                                  {val1.listSubFeeIncurreds.map(
                                                                    (
                                                                      val2,
                                                                      index2
                                                                    ) => {
                                                                      return (
                                                                        <>
                                                                          <tr>
                                                                            <td>
                                                                              {index2 +
                                                                                1}
                                                                            </td>
                                                                            <td>
                                                                              {
                                                                                val2.sfName
                                                                              }
                                                                            </td>
                                                                            <td>
                                                                              {val2.price.toLocaleString(
                                                                                "vi-VI",
                                                                                {
                                                                                  style:
                                                                                    "currency",
                                                                                  currency:
                                                                                    "VND",
                                                                                }
                                                                              )}
                                                                            </td>
                                                                          </tr>
                                                                        </>
                                                                      );
                                                                    }
                                                                  )}
                                                                </tbody>
                                                              </table>
                                                            </td>
                                                          </tr>
                                                        </>
                                                      )}
                                                  </tbody>
                                                </>
                                              );
                                            }
                                          )}
                                      </table>
                                    </td>
                                  </tr>
                                </tbody>
                              </table>
                            </>
                          );
                        })}
                    </table>
                    <div className="row">
                      <div className="col col-8"></div>
                      <div className="col col-4">
                        <div className="row my-2">
                          <div className="col-7 text-right">Tổng Tiền</div>
                          <div className="col-5">
                            <span className="text-120 text-secondary-d1">
                              {!totalSum
                                ? 0
                                : totalSum.total.toLocaleString("vi-VI", {
                                    style: "currency",
                                    currency: "VND",
                                  })}
                            </span>
                          </div>
                        </div>
                        <div className="row my-2">
                          <div className="col-7 text-right">Tổng Phụ Phí</div>
                          <div className="col-5">
                            <span className="text-110 text-secondary-d1">
                              {!totalSum
                                ? 0
                                : totalSum.totalsf.toLocaleString("vi-VI", {
                                    style: "currency",
                                    currency: "VND",
                                  })}
                            </span>
                          </div>
                        </div>
                        <div className="row my-2 align-items-center bgc-primary-l3 p-2">
                          <div className="col-7 text-right">Tổng Hóa Đơn</div>
                          <div className="col-5">
                            <span className="text-150 text-success-d3 opacity-2">
                              {totalSum
                                ? (
                                    totalSum.totalsf + totalSum.total
                                  ).toLocaleString("vi-VI", {
                                    style: "currency",
                                    currency: "VND",
                                  })
                                : 0}
                            </span>
                          </div>
                        </div>
                      </div>
                    </div>
                    <hr />
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
              style={{ maxWidth: "99%" }}
            >
              <div className="modal-content">
                <div className="modal-header">
                  <h5>{titleModal}</h5>
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
                    {ShowModal === "ShowListHandlingPick" && (
                      <ListHandlingToPick
                        cusId={customer}
                        datePay={datePay}
                        hideModal={hideModal}
                        reloadData={handleOnChangeExchangeRate}
                      ></ListHandlingToPick>
                    )}
                  </>
                </div>
              </div>
            </div>
          </div>
          {ShowConfirm === true && (
            <ConfirmDialog
              setShowConfirm={setShowConfirm}
              title={"Bạn có chắc chắn muốn chốt sản lượng?"}
              content={
                "Khi thực hiện hành động này sẽ không thể hoàn tác lại được nữa."
              }
              funcAgree={funcAgree}
            />
          )}
        </div>
      )}
    </>
  );
};

export default ListBillDetail;
