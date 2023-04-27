import { useState, useEffect } from "react";
import { getData } from "../Common/FuncAxios";
import { Modal } from "bootstrap";
import Logo from "../../Image/Logo/logo2x.png";
import "./bill.css";
import { ToastError } from "../Common/FuncToast";
import moment from "moment";

const DetailBillSupplier = (props) => {
  const { supplier, fromDate, toDate } = props;
  const [isLoading, setIsLoading] = useState(true);
  const [dataBill, setDataBill] = useState([]);

  const [totalSum, setTotalSum] = useState({});

  useEffect(() => {
    console.log(supplier);
    if (supplier && fromDate && toDate) {
      setIsLoading(true);

      getDataBill(supplier.value, fromDate, toDate);
    } else {
      ToastError("Vui lòng chọn Khách Hàng, và thời xem để xem");
      return;
    }
  }, [props, fromDate, toDate, supplier]);

  const getDataBill = async (supplierId, fromDate, toDate) => {
    if (supplierId && fromDate && toDate) {
      fromDate = moment(fromDate).format("YYYY-MM-DD");
      toDate = moment(toDate).format("YYYY-MM-DD");

      var dataBill = await getData(
        `Bills/GetBillByCustomerId?customerId=${supplierId}&fromDate=${fromDate}&&toDate=${toDate}`
      );

      if (dataBill.billReuslt && dataBill.billReuslt.length > 0) {
        setDataBill(dataBill.billReuslt);
        var sumData = getTotalPrice(dataBill.billReuslt);
        setTotalSum(sumData);
        setIsLoading(false);
      }
    }
  };

  const getTotalPrice = (billReuslt) => {
    let total = 0;
    let totalsf = 0;
    if (billReuslt && billReuslt.length > 0) {
      billReuslt.forEach((br) => {
        if (br.listHandling && br.listHandling.length > 0) {
          br.listHandling.forEach((hl) => {
            total = total + hl.donGia;

            hl.listSubFeeByContract.forEach((val) => {
              totalsf = totalsf + val.unitPrice;
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
                <div className="page-tools">
                  <div className="action-buttons">
                    <a
                      className="btn bg-white btn-light mx-1px text-95"
                      href="#"
                      data-title="In Hóa Đơn"
                    >
                      <i className="mr-1 fa fa-print text-primary-m1 text-120 w-2" />
                      In Hóa Đơn
                    </a>
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
                        <h1 className="text-default-d3">{supplier.label}</h1>
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
                                    {/* <th scope="col">Account</th> */}
                                    <th scope="col">Loại Vận Đơn</th>
                                    <th scope="col">Điểm Đóng Hàng</th>
                                    <th scope="col">Điểm Hạ Hàng</th>
                                  </tr>
                                </thead>
                                <tbody>
                                  <tr>
                                    <th scope="row"></th>
                                    <td>{val.maVanDonKH}</td>
                                    {/* <td>{val.accountName}</td> */}
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
                                                            <td
                                                              colSpan={1}
                                                            ></td>
                                                            <td colSpan={6}>
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
                                                            <td
                                                              colSpan={1}
                                                            ></td>
                                                            <td colSpan={6}>
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
                              {totalSum.total.toLocaleString("vi-VI", {
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
                              {totalSum.totalsf.toLocaleString("vi-VI", {
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
                              {(
                                totalSum.totalsf + totalSum.total
                              ).toLocaleString("vi-VI", {
                                style: "currency",
                                currency: "VND",
                              })}
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
        </div>
      )}
    </>
  );
};

export default DetailBillSupplier;
