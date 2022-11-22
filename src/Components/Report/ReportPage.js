import { useEffect, useState } from "react";
import { getData } from "../Common/FuncAxios";
import moment from "moment";
import { Modal } from "bootstrap";
import DatePicker from "react-datepicker";
import ChartDate from "../Chart/ChartDate";

const ReportPage = () => {
  const [dataMonthTransport, setDataMonthTransport] = useState([]);
  const [dataMonthRevenue, setDataMonthRevenue] = useState([]);
  const [month, setMonth] = useState();
  const [isLoading, SetIsLoading] = useState(false);
  const [totalT, setTotalT] = useState([]);
  const [totalRevenue, setTotalRevenue] = useState([]);

  useEffect(() => {
    GetDataTransportByMonth(new Date());
    GetDataRevenueByMonth(new Date());
  }, []);

  const GetDataTransportByMonth = async (val) => {
    SetIsLoading(true);
    setMonth(new Date(val));
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
    setMonth(new Date(val));
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
      {/* Content Header (Page header) */}
      <div className="content-header">
        <div className="container-fluid">
          <div className="row mb-2">
            <div className="col-sm-6">
              <h1 className="m-0">Báo Cáo Và Thống Kê</h1>
            </div>
            {/* /.col */}
            <div className="col-sm-6">
              {/* <ol className="breadcrumb float-sm-right">
                <li className="breadcrumb-item">
                  <a href="#">Home</a>
                </li>
                <li className="breadcrumb-item active">Dashboard v3</li>
              </ol> */}
            </div>
            {/* /.col */}
          </div>
          {/* /.row */}
        </div>
        {/* /.container-fluid */}
      </div>
      {/* /.content-header */}
      {/* Main content */}
      <div className="content">
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
                                selected={month}
                                onChange={(date) =>
                                  GetDataTransportByMonth(date)
                                }
                                dateFormat="MM/yyyy"
                                className="form-control form-control-sm"
                                placeholderText="Chọn Tháng"
                                value={month}
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
                  {/* /.d-flex */}
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
                      {isLoading === true ? (
                        <div>Loading Data...</div>
                      ) : (
                        dataMonthTransport &&
                        Object.keys(dataMonthTransport).length > 0 && (
                          <ChartDate
                            arrData={dataMonthTransport}
                            type={"int"}
                          ></ChartDate>
                        )
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
                                selected={month}
                                onChange={(date) => GetDataRevenueByMonth(date)}
                                dateFormat="MM/yyyy"
                                className="form-control form-control-sm"
                                placeholderText="Chọn Tháng"
                                value={month}
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
                  {/* /.d-flex */}
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
                      {isLoading === true ? (
                        <div>Loading Data...</div>
                      ) : (
                        dataMonthRevenue &&
                        Object.keys(dataMonthRevenue).length > 0 && (
                          <ChartDate
                            arrData={dataMonthRevenue}
                            type={"double"}
                          ></ChartDate>
                        )
                      )}
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
          {/* /.row */}
        </div>
        {/* /.container-fluid */}
      </div>
      {/* /.content */}
    </div>
  );
};

export default ReportPage;
