import { useState, useEffect } from "react";
import { getData } from "../Common/FuncAxios";
import { Modal } from "bootstrap";
import Logo from "../../Image/Logo/logo2x.png";
import "./bill.css";

const DetailBillByTransport = (props) => {
  const { dataClick } = props;
  const [dataBill, setDataBill] = useState([]);

  useEffect(() => {
    if (props && dataClick && Object.keys(dataClick).length > 0) {
      let data = getDataBill(dataClick.maKh, dataClick.maVanDon);
    }
  }, [props, dataClick]);

  const getDataBill = async (customerId, transportId) => {
    if (customerId) {
      var dataBill = await getData(
        `Bills/GetBillByTransportId?customerId=${customerId}&transportId=${transportId}`
      );

      if (dataBill.billReuslt && dataBill.billReuslt.length > 0) {
        setDataBill(dataBill.billReuslt);
      }
    }
  };

  const getTotalSubFee = (dataHandling) => {
    let total = 0;
    if (dataHandling && dataHandling.length > 0) {
      dataHandling.map((sc) => {
        if (sc.listSubFeeByContract && sc.listSubFeeByContract.length > 0) {
          sc.listSubFeeByContract.map((sfc) => {
            total += parseFloat(sfc.unitPrice);
          });
        }
        if (sc.listSubFeeIncurreds && sc.listSubFeeIncurreds.length > 0) {
          sc.listSubFeeIncurreds.map((sfi) => {
            total += parseFloat(sfi.price);
          });
        }
      });
    }
    return total;
  };

  const getTotalRoute = (dataRoute) => {
    let total = dataRoute.reduce((acc, o) => acc + parseFloat(o.donGia), 0);
    return total;
  };

  return (
    <>
      <div>
        <div className="page-content">
          {/* <div className="page-header text-blue-d2">
            <h1 className="page-title text-secondary-d1">
              <img src={Logo}></img>
            </h1>
            <div className="page-tools">
              <div className="action-buttons">
                <a
                  className="btn bg-white btn-light mx-1px text-95"
                  href="#"
                  data-title="Print"
                >
                  <i className="mr-1 fa fa-print text-primary-m1 text-120 w-2" />
                  Print
                </a>
                <a
                  className="btn bg-white btn-light mx-1px text-95"
                  href="#"
                  data-title="PDF"
                >
                  <i className="mr-1 fa fa-file-pdf-o text-danger-m1 text-120 w-2" />
                  Export
                </a>
              </div>
            </div>
          </div> */}
          <div className="col-12">
            <div className="row mt-4">
              <div className="col-12 col-lg-12">
                <div className="row">
                  <div className="col-12">
                    <div className="text-center text-150">
                      <h1 className="text-default-d3">{dataClick.khachHang}</h1>
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
                  <table
                    className="table table-striped table-bordered table-hover table-sm table-responsive-sm"
                    style={{ border: "3px solid" }}
                  >
                    {dataBill &&
                      dataBill.length > 0 &&
                      dataBill.map((val, index) => {
                        return (
                          <>
                            <thead>
                              <tr>
                                <th scope="col">{index + 1}</th>
                                <th scope="col">Mã Vận Đơn</th>
                                <th scope="col">Loại Vận Đơn</th>
                                <th scope="col">Cung Đường</th>
                                <th scope="col">Tổng Khối Lượng</th>
                                <th scope="col">Tổng Thể Tích</th>
                              </tr>
                            </thead>
                            <tbody>
                              <tr>
                                <th scope="row"></th>
                                <td>{val.maVanDonKH}</td>
                                <td>{val.loaiVanDon}</td>
                                <td>{val.tenCungDuong}</td>
                                <td>{val.tongKhoiLuong}</td>
                                <td>{val.tongTheTich}</td>
                              </tr>

                              <tr>
                                <td colSpan={2}></td>
                                <td colSpan={4}>
                                  <table className="table table-striped table-bordered table-hover table-sm table-responsive-sm">
                                    <thead>
                                      <tr>
                                        <th>Chuyến</th>
                                        <th>Điểm Lấy Rỗng</th>
                                        <th>Đơn Vị Tính</th>
                                        <th>Đơn Vị Vận Tải</th>
                                        <th>Loại Hàng Hóa</th>
                                        <th>Loại Phương Tiện</th>
                                        <th>Đơn Giá Tuyến</th>
                                      </tr>
                                    </thead>
                                    <tbody>
                                      {val.listHandling &&
                                        val.listHandling.length > 0 &&
                                        val.listHandling.map((val1, index1) => {
                                          return (
                                            <>
                                              <tr>
                                                <th>{index1 + 1}</th>
                                                <td>{val1.diemLayRong}</td>
                                                <td>{val1.donViTinh}</td>
                                                <td>{val1.donViVanTai}</td>
                                                <td>{val1.loaiHangHoa}</td>
                                                <td>{val1.loaiPhuongTien}</td>
                                                <td>
                                                  {val1.donGia.toLocaleString(
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
                                                      <th
                                                        colSpan={7}
                                                        style={{
                                                          textAlign: "center",
                                                        }}
                                                      >
                                                        # Phụ Phí Theo Hợp Đồng
                                                        #
                                                      </th>
                                                    </tr>
                                                    <tr>
                                                      <td colSpan={1}></td>
                                                      <td colSpan={6}>
                                                        <table className="table table-striped table-bordered table-hover table-sm table-responsive-sm">
                                                          <thead>
                                                            <tr>
                                                              <th></th>
                                                              <th>
                                                                Tên Phụ Phí
                                                              </th>
                                                              <th>Đơn Giá</th>
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
                                                                        {val2.unitPrice.toLocaleString(
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
                                                      <th
                                                        colSpan={7}
                                                        style={{
                                                          textAlign: "center",
                                                        }}
                                                      >
                                                        # Phụ Phí Phát Sinh #
                                                      </th>
                                                    </tr>
                                                    <tr>
                                                      <td colSpan={1}></td>
                                                      <td colSpan={6}>
                                                        <table className="table table-striped table-bordered table-hover table-sm table-responsive-sm">
                                                          <thead>
                                                            <tr>
                                                              <th></th>
                                                              <th>
                                                                Tên Phụ Phí
                                                              </th>
                                                              <th>Đơn Giá</th>
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
                                            </>
                                          );
                                        })}
                                    </tbody>
                                  </table>
                                </td>
                              </tr>
                              <tr>
                                <th colSpan={5} style={{ textAlign: "right" }}>
                                  Tổng Tiền Chuyến
                                </th>
                                <th style={{ textAlign: "right" }}>
                                  {getTotalRoute(
                                    val.listHandling
                                  ).toLocaleString("vi-VI", {
                                    style: "currency",
                                    currency: "VND",
                                  })}
                                </th>
                              </tr>
                              <tr>
                                <th colSpan={5} style={{ textAlign: "right" }}>
                                  Tổng Phụ Phí
                                </th>
                                <th style={{ textAlign: "right" }}>
                                  {getTotalSubFee(
                                    val.listHandling
                                  ).toLocaleString("vi-VI", {
                                    style: "currency",
                                    currency: "VND",
                                  })}
                                </th>
                              </tr>
                              <tr>
                                <th colSpan={5} style={{ textAlign: "right" }}>
                                  Tổng Tiền
                                </th>
                                <th style={{ textAlign: "right" }}>
                                  {(
                                    getTotalRoute(val.listHandling) +
                                    getTotalSubFee(val.listHandling)
                                  ).toLocaleString("vi-VI", {
                                    style: "currency",
                                    currency: "VND",
                                  })}
                                </th>
                              </tr>
                            </tbody>
                          </>
                        );
                      })}
                  </table>

                  {/* <div className="row mt-3">
                    <div className="col-12 col-sm-7 text-grey-d2 text-95 mt-2 mt-lg-0">
                      Extra note such as company or payment information...
                    </div>
                    <div className="col-12 col-sm-5 text-grey text-90 order-first order-sm-last">
                      <div className="row my-2">
                        <div className="col-7 text-right">SubTotal</div>
                        <div className="col-5">
                          <span className="text-120 text-secondary-d1">
                            $2,250
                          </span>
                        </div>
                      </div>
                      <div className="row my-2">
                        <div className="col-7 text-right">Tax (10%)</div>
                        <div className="col-5">
                          <span className="text-110 text-secondary-d1">
                            $225
                          </span>
                        </div>
                      </div>
                      <div className="row my-2 align-items-center bgc-primary-l3 p-2">
                        <div className="col-7 text-right">Total Amount</div>
                        <div className="col-5">
                          <span className="text-150 text-success-d3 opacity-2">
                            $2,475
                          </span>
                        </div>
                      </div>
                    </div>
                  </div>
                  <hr />
                  <div>
                    <span className="text-secondary-d1 text-105">
                      Thank you for your business
                    </span>
                    <a
                      href="#"
                      className="btn btn-info btn-bold px-4 float-right mt-3 mt-lg-0"
                    >
                      Pay Now
                    </a>
                  </div> */}
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </>
  );
};

export default DetailBillByTransport;
