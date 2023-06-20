import React, { useEffect, useState } from "react";
import { getData } from "../Common/FuncAxios";
import moment from "moment";
import { Modal } from "bootstrap";
import DatePicker from "react-datepicker";
import ChartDate from "../Chart/ChartDate";
import LoadingPage from "../Common/Loading/LoadingPage";

const ReportPage = () => {
  const [dataMonthTransport, setDataMonthTransport] = useState([]);
  const [dataMonthRevenue, setDataMonthRevenue] = useState([]);

  const [monthRevenue, setMonthRevenue] = useState();
  const [monthTransport, setMonthTransport] = useState();
  const [monthKy, setMonthKy] = useState();

  const [isLoading, SetIsLoading] = useState(false);
  const [totalT, setTotalT] = useState([]);
  const [totalRevenue, setTotalRevenue] = useState([]);

  const [dataTransportReport, setDataTransportReport] = useState({});

  // const [dateRange, setDateRange] = useState([
  //   new Date(moment().startOf("month")),
  //   new Date(moment().endOf("month")),
  // ]);
  // const [startDate, endDate] = dateRange;

  useEffect(() => {
    GetDataTransportByMonth(new Date());
    GetDataRevenueByMonth(new Date());
    loadTransportReport(new Date());
  }, []);

  const loadTransportReport = async (value) => {
    SetIsLoading(true);
    setMonthKy(new Date(value));

    // let fromDate = moment(new Date(value[0])).format("YYYY-MM-DD");
    // let toDate = moment(new Date(value[1])).format("YYYY-MM-DD");
    const getdata = await getData(
      `Report/GetCustomerReport?dateTime=${moment(new Date(value)).format(
        "YYYY-MM-DD"
      )}`
    );
    setDataTransportReport(getdata);
    SetIsLoading(false);
  };

  const GetDataTransportByMonth = async (val) => {
    SetIsLoading(true);
    setMonthTransport(new Date(val));
    const GetReportTransportByMonth = await getData(
      `Report/GetReportTransportByMonth?dateTime=${moment(new Date(val)).format(
        "YYYY-MM-DD"
      )}`
    );
    if (
      GetReportTransportByMonth &&
      Object.keys(GetReportTransportByMonth).length > 0
    ) {
      setDataMonthTransport(GetReportTransportByMonth);
      setTotalT(GetReportTransportByMonth.totalReports);
    } else {
      setDataMonthTransport([]);
      setTotalT([]);
    }

    SetIsLoading(false);
  };

  const GetDataRevenueByMonth = async (val) => {
    SetIsLoading(true);
    setMonthRevenue(new Date(val));
    const GetReportRevenueByMonth = await getData(
      `Report/GetReportRevenue?dateTime=${moment(new Date(val)).format(
        "YYYY-MM-DD"
      )}`
    );
    if (
      GetReportRevenueByMonth &&
      Object.keys(GetReportRevenueByMonth).length > 0
    ) {
      setDataMonthRevenue(GetReportRevenueByMonth);
      setTotalRevenue(GetReportRevenueByMonth.totalReports);
    } else {
      setDataMonthRevenue([]);
      setTotalRevenue([]);
    }
    SetIsLoading(false);
  };

  return (
    <div className="content-wrapper">
      <div className="content-header">
        <div className="container-fluid">
          <div className="row mb-2">
            <div className="col-sm-6">
              <h1 className="m-0">Báo Cáo Và Thống Kê</h1>
            </div>
            <div className="col-sm-6">
              {/* <ol className="breadcrumb float-sm-right">
                <li className="breadcrumb-item">
                  <a href="#">Home</a>
                </li>
                <li className="breadcrumb-item active">Dashboard v3</li>
              </ol> */}
            </div>
          </div>
        </div>
      </div>
      {isLoading && isLoading === true ? (
        <div>
          <LoadingPage></LoadingPage>
        </div>
      ) : (
        <div className="content">
          <div className="container-fluid">
            <div className="row">
              <div className="col-sm">
                <div className="card">
                  <div className="card-header border-0">
                    <div className="d-flex justify-content-between">
                      <div className="col col-12">
                        <div className="row">
                          <div className="col col-8">
                            <h3 className="card-title text-bold text-lg">
                              Thống kê chuyến theo tháng
                            </h3>
                          </div>
                          <div className="col-sm-3">
                            <div className="form-group">
                              <div className="row">
                                <div className="form-group">
                                  <div className="row">
                                    <div className="col col-sm">
                                      <label htmlFor="month">Chọn tháng:</label>
                                    </div>
                                    <div className="col col-sm">
                                      <DatePicker
                                        selected={monthKy}
                                        onChange={(date) =>
                                          loadTransportReport(date)
                                        }
                                        dateFormat="MM/yyyy"
                                        className="form-control form-control-sm"
                                        placeholderText="Chọn Tháng"
                                        value={monthKy}
                                        showMonthYearPicker
                                        showFullMonthYearPicker
                                      />
                                    </div>
                                  </div>
                                </div>
                                {/* <div className="col col-sm">
                                  <label>Chọn Tháng:</label>
                                </div> */}

                                {/* <div className="col col-sm">
                                  <DatePicker
                                    className="form-control form-control-sm"
                                    dateFormat="dd/MM/yyyy"
                                    selectsRange={true}
                                    startDate={startDate}
                                    endDate={endDate}
                                    onChange={(update) => {
                                      loadTransportReport(update);
                                    }}
                                    value={dateRange}
                                    withPortal
                                  />
                                </div> */}
                              </div>
                            </div>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                  <div className="card-body">
                    <div className="row">
                      <div className="col-12">
                        <div className="card">
                          <div className="card-header">
                            <h3 className="card-title">
                              Thống kê dữ liệu của khách hàng
                            </h3>
                          </div>
                          <div
                            className="card-body table-responsive p-0 overflow-auto "
                            style={{ maxHeight: "50vh" }}
                          >
                            <table className="table table-hover text-nowrap table-bordered">
                              <thead className="bg-light sticky-top top-0">
                                <tr>
                                  <th>Khách Hàng</th>
                                  <th>Tổng Số Booking</th>
                                  <th>Tổng Số Chuyến</th>
                                  <th>20</th>
                                  <th>40</th>
                                  <th>40RF</th>
                                  <th>45</th>
                                  <th>1T</th>
                                  <th>1.5T</th>
                                  <th>1.7T</th>
                                  <th>2T</th>
                                  <th>2.5T</th>
                                  <th>3T</th>
                                  <th>3.5T</th>
                                  <th>5T</th>
                                  <th>7T</th>
                                  <th>8T</th>
                                  <th>9T</th>
                                  <th>10T</th>
                                  <th>15T</th>
                                  <th>Doanh Thu</th>
                                  <th>Chi Phí</th>
                                  <th>Lợi Nhuận</th>
                                  <th>Tỉ Xuất LN/CP</th>
                                </tr>
                              </thead>
                              <tbody>
                                {dataTransportReport &&
                                  Object.keys(dataTransportReport).length >
                                    0 && (
                                    <>
                                      {dataTransportReport.customerReports &&
                                        dataTransportReport.customerReports
                                          .length > 0 &&
                                        dataTransportReport.customerReports.map(
                                          (val, index) => {
                                            return (
                                              <tr key={index}>
                                                <td>{val.customerName}</td>
                                                <td>{val.totalBooking}</td>
                                                <td>{val.total}</td>
                                                <td>{val.conT20}</td>
                                                <td>{val.conT40}</td>
                                                <td>{val.conT40RF}</td>
                                                <td>{val.conT45}</td>
                                                <td>{val.trucK1}</td>
                                                <td>{val.trucK15}</td>
                                                <td>{val.trucK17}</td>
                                                <td>{val.trucK2}</td>
                                                <td>{val.trucK25}</td>
                                                <td>{val.trucK3}</td>
                                                <td>{val.trucK35}</td>
                                                <td>{val.trucK5}</td>
                                                <td>{val.trucK7}</td>
                                                <td>{val.trucK8}</td>
                                                <td>{val.trucK9}</td>
                                                <td>{val.trucK10}</td>
                                                <td>{val.trucK150}</td>
                                                <td>
                                                  {val.totalMoney.toLocaleString(
                                                    "vi-VI",
                                                    {
                                                      style: "currency",
                                                      currency: "VND",
                                                    }
                                                  )}
                                                </td>
                                                <td>
                                                  {val.totalSf.toLocaleString(
                                                    "vi-VI",
                                                    {
                                                      style: "currency",
                                                      currency: "VND",
                                                    }
                                                  )}
                                                </td>
                                                <td>
                                                  {val.profit.toLocaleString(
                                                    "vi-VI",
                                                    {
                                                      style: "currency",
                                                      currency: "VND",
                                                    }
                                                  )}
                                                </td>
                                                <td>
                                                  {Math.round(
                                                    (val.profit / val.totalSf) *
                                                      100
                                                  )}
                                                  %
                                                </td>
                                              </tr>
                                            );
                                          }
                                        )}
                                      <tr style={{ border: "3px solid" }}>
                                        <td
                                          style={{
                                            textAlign: "center",
                                            fontWeight: "bold",
                                          }}
                                        >
                                          Tổng Tính:
                                        </td>
                                        <td>
                                          {dataTransportReport.customerReports.reduce(
                                            (acc, val) =>
                                              acc +
                                              parseFloat(val.totalBooking),
                                            0
                                          )}
                                        </td>
                                        <td>
                                          {dataTransportReport.customerReports.reduce(
                                            (acc, val) =>
                                              acc + parseFloat(val.total),
                                            0
                                          )}
                                        </td>
                                        <td>
                                          {dataTransportReport.customerReports.reduce(
                                            (acc, val) =>
                                              acc + parseFloat(val.conT20),
                                            0
                                          )}
                                        </td>
                                        <td>
                                          {dataTransportReport.customerReports.reduce(
                                            (acc, val) =>
                                              acc + parseFloat(val.conT40),
                                            0
                                          )}
                                        </td>
                                        <td>
                                          {dataTransportReport.customerReports.reduce(
                                            (acc, val) =>
                                              acc + parseFloat(val.conT40RF),
                                            0
                                          )}
                                        </td>
                                        <td>
                                          {dataTransportReport.customerReports.reduce(
                                            (acc, val) =>
                                              acc + parseFloat(val.conT45),
                                            0
                                          )}
                                        </td>
                                        <td>
                                          {dataTransportReport.customerReports.reduce(
                                            (acc, val) =>
                                              acc + parseFloat(val.trucK1),
                                            0
                                          )}
                                        </td>
                                        <td>
                                          {dataTransportReport.customerReports.reduce(
                                            (acc, val) =>
                                              acc + parseFloat(val.trucK15),
                                            0
                                          )}
                                        </td>
                                        <td>
                                          {dataTransportReport.customerReports.reduce(
                                            (acc, val) =>
                                              acc + parseFloat(val.trucK17),
                                            0
                                          )}
                                        </td>
                                        <td>
                                          {dataTransportReport.customerReports.reduce(
                                            (acc, val) =>
                                              acc + parseFloat(val.trucK2),
                                            0
                                          )}
                                        </td>
                                        <td>
                                          {dataTransportReport.customerReports.reduce(
                                            (acc, val) =>
                                              acc + parseFloat(val.trucK25),
                                            0
                                          )}
                                        </td>
                                        <td>
                                          {dataTransportReport.customerReports.reduce(
                                            (acc, val) =>
                                              acc + parseFloat(val.trucK3),
                                            0
                                          )}
                                        </td>
                                        <td>
                                          {dataTransportReport.customerReports.reduce(
                                            (acc, val) =>
                                              acc + parseFloat(val.trucK35),
                                            0
                                          )}
                                        </td>
                                        <td>
                                          {dataTransportReport.customerReports.reduce(
                                            (acc, val) =>
                                              acc + parseFloat(val.trucK5),
                                            0
                                          )}
                                        </td>
                                        <td>
                                          {dataTransportReport.customerReports.reduce(
                                            (acc, val) =>
                                              acc + parseFloat(val.trucK7),
                                            0
                                          )}
                                        </td>
                                        <td>
                                          {dataTransportReport.customerReports.reduce(
                                            (acc, val) =>
                                              acc + parseFloat(val.trucK8),
                                            0
                                          )}
                                        </td>
                                        <td>
                                          {dataTransportReport.customerReports.reduce(
                                            (acc, val) =>
                                              acc + parseFloat(val.trucK9),
                                            0
                                          )}
                                        </td>
                                        <td>
                                          {dataTransportReport.customerReports.reduce(
                                            (acc, val) =>
                                              acc + parseFloat(val.trucK10),
                                            0
                                          )}
                                        </td>
                                        <td>
                                          {dataTransportReport.customerReports.reduce(
                                            (acc, val) =>
                                              acc + parseFloat(val.trucK150),
                                            0
                                          )}
                                        </td>

                                        <td>
                                          {dataTransportReport.customerReports
                                            .reduce(
                                              (acc, val) =>
                                                acc +
                                                parseFloat(val.totalMoney),
                                              0
                                            )
                                            .toLocaleString("vi-VI", {
                                              style: "currency",
                                              currency: "VND",
                                            })}
                                        </td>
                                        <td>
                                          {dataTransportReport.customerReports
                                            .reduce(
                                              (acc, val) =>
                                                acc + parseFloat(val.totalSf),
                                              0
                                            )
                                            .toLocaleString("vi-VI", {
                                              style: "currency",
                                              currency: "VND",
                                            })}
                                        </td>
                                        <td>
                                          {dataTransportReport.customerReports
                                            .reduce(
                                              (acc, val) =>
                                                acc + parseFloat(val.profit),
                                              0
                                            )
                                            .toLocaleString("vi-VI", {
                                              style: "currency",
                                              currency: "VND",
                                            })}
                                        </td>
                                        <td>
                                          {Math.round(
                                            (dataTransportReport.customerReports.reduce(
                                              (acc, val) =>
                                                acc + parseFloat(val.profit),
                                              0
                                            ) /
                                              dataTransportReport.customerReports.reduce(
                                                (acc, val) =>
                                                  acc + parseFloat(val.totalSf),
                                                0
                                              )) *
                                              100
                                          )}
                                          %
                                        </td>
                                      </tr>
                                    </>
                                  )}
                              </tbody>
                            </table>
                          </div>
                        </div>
                      </div>
                    </div>
                    <div className="row">
                      <div className="col-12">
                        <div className="card">
                          <div className="card-header">
                            <h3 className="card-title">
                              Thống kê dữ liệu của nhà cung cấp
                            </h3>
                          </div>
                          <div
                            className="card-body table-responsive p-0 overflow-auto "
                            style={{ maxHeight: "50vh" }}
                          >
                            <table className="table table-hover text-nowrap table-bordered">
                              <thead className="bg-light sticky-top top-0">
                                <tr>
                                  <th>Tên Nhà Cung Cấp</th>
                                  <th>Tổng Số Chuyến</th>
                                  <th>20</th>
                                  <th>40</th>
                                  <th>40RF</th>
                                  <th>45</th>
                                  <th>1T</th>
                                  <th>1.5T</th>
                                  <th>1.7T</th>
                                  <th>2T</th>
                                  <th>2.5T</th>
                                  <th>3T</th>
                                  <th>3.5T</th>
                                  <th>5T</th>
                                  <th>7T</th>
                                  <th>8T</th>
                                  <th>9T</th>
                                  <th>10T</th>
                                  <th>15T</th>
                                  <th>Doanh Thu</th>
                                  <th>Chi Phí</th>
                                  <th>Lợi Nhuận</th>
                                  <th>Tỉ Xuất LN/CP</th>
                                </tr>
                              </thead>
                              <tbody>
                                {dataTransportReport &&
                                  Object.keys(dataTransportReport).length >
                                    0 && (
                                    <>
                                      {dataTransportReport.supllierReports &&
                                        dataTransportReport.supllierReports
                                          .length > 0 &&
                                        dataTransportReport.supllierReports.map(
                                          (val, index) => {
                                            return (
                                              <tr key={index}>
                                                <td>{val.customerName}</td>
                                                <td>{val.total}</td>
                                                <td>{val.conT20}</td>
                                                <td>{val.conT40}</td>
                                                <td>{val.conT40RF}</td>
                                                <td>{val.conT45}</td>
                                                <td>{val.trucK1}</td>
                                                <td>{val.trucK15}</td>
                                                <td>{val.trucK17}</td>
                                                <td>{val.trucK2}</td>
                                                <td>{val.trucK25}</td>
                                                <td>{val.trucK3}</td>
                                                <td>{val.trucK35}</td>
                                                <td>{val.trucK5}</td>
                                                <td>{val.trucK7}</td>
                                                <td>{val.trucK8}</td>
                                                <td>{val.trucK9}</td>
                                                <td>{val.trucK10}</td>
                                                <td>{val.trucK150}</td>
                                                <td>
                                                  {val.totalMoney.toLocaleString(
                                                    "vi-VI",
                                                    {
                                                      style: "currency",
                                                      currency: "VND",
                                                    }
                                                  )}
                                                </td>
                                                <td>
                                                  {val.totalSf.toLocaleString(
                                                    "vi-VI",
                                                    {
                                                      style: "currency",
                                                      currency: "VND",
                                                    }
                                                  )}
                                                </td>
                                                <td>
                                                  {val.profit.toLocaleString(
                                                    "vi-VI",
                                                    {
                                                      style: "currency",
                                                      currency: "VND",
                                                    }
                                                  )}
                                                </td>
                                                <td>
                                                  {Math.round(
                                                    (val.profit / val.totalSf) *
                                                      100
                                                  )}
                                                  %
                                                </td>
                                              </tr>
                                            );
                                          }
                                        )}
                                      <tr style={{ border: "3px solid" }}>
                                        <td
                                          style={{
                                            textAlign: "center",
                                            fontWeight: "bold",
                                          }}
                                        >
                                          Tổng Tính:
                                        </td>
                                        <td>
                                          {dataTransportReport.supllierReports.reduce(
                                            (acc, val) =>
                                              acc + parseFloat(val.total),
                                            0
                                          )}
                                        </td>
                                        <td>
                                          {dataTransportReport.supllierReports.reduce(
                                            (acc, val) =>
                                              acc + parseFloat(val.conT20),
                                            0
                                          )}
                                        </td>
                                        <td>
                                          {dataTransportReport.supllierReports.reduce(
                                            (acc, val) =>
                                              acc + parseFloat(val.conT40),
                                            0
                                          )}
                                        </td>
                                        <td>
                                          {dataTransportReport.supllierReports.reduce(
                                            (acc, val) =>
                                              acc + parseFloat(val.conT40RF),
                                            0
                                          )}
                                        </td>
                                        <td>
                                          {dataTransportReport.supllierReports.reduce(
                                            (acc, val) =>
                                              acc + parseFloat(val.conT45),
                                            0
                                          )}
                                        </td>
                                        <td>
                                          {dataTransportReport.supllierReports.reduce(
                                            (acc, val) =>
                                              acc + parseFloat(val.trucK1),
                                            0
                                          )}
                                        </td>
                                        <td>
                                          {dataTransportReport.supllierReports.reduce(
                                            (acc, val) =>
                                              acc + parseFloat(val.trucK15),
                                            0
                                          )}
                                        </td>
                                        <td>
                                          {dataTransportReport.supllierReports.reduce(
                                            (acc, val) =>
                                              acc + parseFloat(val.trucK17),
                                            0
                                          )}
                                        </td>
                                        <td>
                                          {dataTransportReport.supllierReports.reduce(
                                            (acc, val) =>
                                              acc + parseFloat(val.trucK2),
                                            0
                                          )}
                                        </td>
                                        <td>
                                          {dataTransportReport.supllierReports.reduce(
                                            (acc, val) =>
                                              acc + parseFloat(val.trucK25),
                                            0
                                          )}
                                        </td>
                                        <td>
                                          {dataTransportReport.supllierReports.reduce(
                                            (acc, val) =>
                                              acc + parseFloat(val.trucK3),
                                            0
                                          )}
                                        </td>
                                        <td>
                                          {dataTransportReport.supllierReports.reduce(
                                            (acc, val) =>
                                              acc + parseFloat(val.trucK35),
                                            0
                                          )}
                                        </td>
                                        <td>
                                          {dataTransportReport.supllierReports.reduce(
                                            (acc, val) =>
                                              acc + parseFloat(val.trucK5),
                                            0
                                          )}
                                        </td>
                                        <td>
                                          {dataTransportReport.supllierReports.reduce(
                                            (acc, val) =>
                                              acc + parseFloat(val.trucK7),
                                            0
                                          )}
                                        </td>
                                        <td>
                                          {dataTransportReport.supllierReports.reduce(
                                            (acc, val) =>
                                              acc + parseFloat(val.trucK8),
                                            0
                                          )}
                                        </td>
                                        <td>
                                          {dataTransportReport.supllierReports.reduce(
                                            (acc, val) =>
                                              acc + parseFloat(val.trucK9),
                                            0
                                          )}
                                        </td>
                                        <td>
                                          {dataTransportReport.supllierReports.reduce(
                                            (acc, val) =>
                                              acc + parseFloat(val.trucK10),
                                            0
                                          )}
                                        </td>
                                        <td>
                                          {dataTransportReport.supllierReports.reduce(
                                            (acc, val) =>
                                              acc + parseFloat(val.trucK150),
                                            0
                                          )}
                                        </td>

                                        <td>
                                          {dataTransportReport.supllierReports
                                            .reduce(
                                              (acc, val) =>
                                                acc +
                                                parseFloat(val.totalMoney),
                                              0
                                            )
                                            .toLocaleString("vi-VI", {
                                              style: "currency",
                                              currency: "VND",
                                            })}
                                        </td>
                                        <td>
                                          {dataTransportReport.supllierReports
                                            .reduce(
                                              (acc, val) =>
                                                acc + parseFloat(val.totalSf),
                                              0
                                            )
                                            .toLocaleString("vi-VI", {
                                              style: "currency",
                                              currency: "VND",
                                            })}
                                        </td>
                                        <td>
                                          {dataTransportReport.supllierReports
                                            .reduce(
                                              (acc, val) =>
                                                acc + parseFloat(val.profit),
                                              0
                                            )
                                            .toLocaleString("vi-VI", {
                                              style: "currency",
                                              currency: "VND",
                                            })}
                                        </td>
                                        <td>
                                          {Math.round(
                                            (dataTransportReport.supllierReports.reduce(
                                              (acc, val) =>
                                                acc + parseFloat(val.profit),
                                              0
                                            ) /
                                              dataTransportReport.supllierReports.reduce(
                                                (acc, val) =>
                                                  acc + parseFloat(val.totalSf),
                                                0
                                              )) *
                                              100
                                          )}
                                          %
                                        </td>
                                      </tr>
                                    </>
                                  )}
                              </tbody>
                            </table>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
          <div className="container-fluid">
            <div className="row">
              <div className="col-sm-6">
                <div className="card">
                  <div className="card-header border-0">
                    <div className="d-flex justify-content-between">
                      <div className="col col-12">
                        <div className="row">
                          <div className="col col-8">
                            <h3 className="card-title text-bold text-lg">
                              Thống kê vận đơn và chuyến theo tháng
                            </h3>
                          </div>
                          <div className="form-group">
                            <div className="row">
                              <div className="col col-sm">
                                <label htmlFor="month">Chọn Tháng:</label>
                              </div>
                              <div className="col col-sm">
                                <DatePicker
                                  selected={monthTransport}
                                  onChange={(date) => {
                                    GetDataTransportByMonth(date);
                                  }}
                                  dateFormat="MM/yyyy"
                                  className="form-control form-control-sm"
                                  placeholderText="Chọn Tháng"
                                  value={monthTransport}
                                  showMonthYearPicker
                                  showFullMonthYearPicker
                                />
                              </div>
                            </div>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                  <div className="card-body">
                    <div className="d-flex">
                      <p className="d-flex flex-column">
                        {totalT &&
                          totalT.length > 0 &&
                          totalT.map((val, index) => {
                            return (
                              <span key={index} className="text-bold">
                                {val.title}: {val.totalInt}
                              </span>
                            );
                          })}
                      </p>
                    </div>
                    <div className="position-relative mb-4">
                      <div className="chartjs-size-monitor">
                        <div className="chartjs-size-monitor-expand">
                          <div />
                        </div>
                        <div className="chartjs-size-monitor-shrink">
                          <div />
                        </div>
                      </div>
                      <div>
                        {dataMonthTransport &&
                          Object.keys(dataMonthTransport).length > 0 && (
                            <ChartDate
                              arrData={dataMonthTransport}
                              type={"int"}
                            ></ChartDate>
                          )}
                      </div>
                    </div>
                  </div>
                </div>
              </div>
              <div className="col-sm-6">
                <div className="card">
                  <div className="card-header border-0">
                    <div className="d-flex justify-content-between">
                      <div className="col col-12">
                        <div className="row">
                          <div className="col col-8">
                            <h3 className="card-title text-bold text-lg">
                              Thống kê Chi Phí,Lợi Nhuận, Doanh Thu Theo Tháng
                            </h3>
                          </div>
                          <div className="form-group">
                            <div className="row">
                              <div className="col col-sm">
                                <label htmlFor="month">Chọn Tháng:</label>
                              </div>
                              <div className="col col-sm">
                                <DatePicker
                                  selected={monthRevenue}
                                  onChange={(date) => {
                                    GetDataRevenueByMonth(date);
                                  }}
                                  dateFormat="MM/yyyy"
                                  className="form-control form-control-sm"
                                  placeholderText="Chọn Tháng"
                                  value={monthRevenue}
                                  showMonthYearPicker
                                  showFullMonthYearPicker
                                />
                              </div>
                            </div>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                  <div className="card-body">
                    <div className="d-flex">
                      <p className="d-flex flex-column">
                        {totalRevenue &&
                          totalRevenue.length > 0 &&
                          totalRevenue.map((val, index) => {
                            return (
                              <span key={index} className="text-bold">
                                {val.title}:{" "}
                                {val.totalDouble.toLocaleString("vi-VI", {
                                  style: "currency",
                                  currency: "VND",
                                })}
                              </span>
                            );
                          })}
                      </p>
                    </div>
                    <div className="position-relative mb-4">
                      <div className="chartjs-size-monitor">
                        <div className="chartjs-size-monitor-expand">
                          <div />
                        </div>
                        <div className="chartjs-size-monitor-shrink">
                          <div />
                        </div>
                      </div>
                      <div>
                        {dataMonthRevenue &&
                          Object.keys(dataMonthRevenue).length > 0 && (
                            <ChartDate
                              arrData={dataMonthRevenue}
                              type={"double"}
                            ></ChartDate>
                          )}
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default ReportPage;
