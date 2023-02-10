import { useState, useEffect, useMemo } from "react";
import { getData } from "../Common/FuncAxios";
import DataTable from "react-data-table-component";
import moment from "moment";

const PriceListContract = (props) => {
  const { selectIdClick, onlyCT, title } = props;

  const columns = useMemo(() => [
    {
      omit: true,
      selector: (row) => row.id,
    },
    {
      name: <div>Mã Khách Hàng</div>,
      selector: (row) => <div className="text-warp">{row.maKh}</div>,
      sortable: true,
    },
    {
      name: <div>Tên Khách Hàng</div>,
      selector: (row) => <div className="text-warp">{row.tenKH}</div>,
      sortable: true,
    },
    {
      name: <div>Mã Hợp Đồng</div>,
      selector: (row) => <div className="text-warp">{row.maHopDong}</div>,
      sortable: true,
    },
    {
      name: "",
      selector: (row) => <div className="text-warp">{row.soHopDongCha}</div>,
      sortable: true,
    },
    {
      name: <div>Mã Cung Đường</div>,
      selector: (row) => <div className="text-warp">{row.maCungDuong}</div>,
      sortable: true,
    },
    {
      name: <div>Đơn Giá</div>,
      selector: (row) => (
        <div className="text-warp">
          {row.donGia.toLocaleString("vi-VI", {
            style: "currency",
            currency: "VND",
          })}
        </div>
      ),
      sortable: true,
    },
    {
      name: <div>Loại Phương Tiện</div>,
      selector: (row) => (
        <div className="text-warp">{row.maLoaiPhuongTien}</div>
      ),
      sortable: true,
    },
    {
      name: <div>Loại Hàng Hóa</div>,
      selector: (row) => <div className="text-warp">{row.maLoaiHangHoa}</div>,
      sortable: true,
    },
    {
      name: <div>Đơn Vị Tính</div>,
      selector: (row) => <div className="text-warp">{row.maDVT}</div>,
      sortable: true,
    },
    // {
    //   name: "Phương Thức Vận Chuyển",
    //   selector: (row) => row.maPTVC,
    //   sortable: true,
    // },
    {
      name: <div>Ngày Áp Dụng</div>,
      selector: (row) => (
        <div className="text-warp">
          {moment(row.ngayApDung).format("DD/MM/YYYY")}
        </div>
      ),
      sortable: true,
    },
    {
      name: <div>Ngày Hết Hiệu Lực</div>,
      selector: (row) =>
        !row.ngayHetHieuLuc ? (
          ""
        ) : (
          <div className="text-warp">{row.ngayHetHieuLuc}</div>
        ),
      sortable: true,
    },
  ]);

  const [IsLoading, SetIsLoading] = useState(false);
  const [selectedId, setSelectedId] = useState({});
  const [data, setData] = useState([]);
  const [totalRows, setTotalRows] = useState(0);
  const [perPage, setPerPage] = useState(10);

  useEffect(() => {
    if (
      props &&
      selectIdClick &&
      title &&
      Object.keys(selectIdClick).length > 0
    ) {
      setSelectedId(selectIdClick);
    }
  }, [props, selectIdClick, title]);

  useEffect(() => {
    if (selectedId && Object.keys(selectedId).length > 0) {
      fetchData(1);
    }
  }, [selectedId]);

  const handlePerRowsChange = async (newPerPage, page) => {
    SetIsLoading(true);
    if (selectedId && Object.keys(selectedId).length > 0) {
      const dataCus = await getData(
        `PriceTable/GetListPriceTableByContractId?Id=${selectedId.maHopDong}&PageNumber=${page}&PageSize=${newPerPage}&onlyct=${onlyCT}`
      );

      setData(dataCus.data);
      setPerPage(newPerPage);
      setTotalRows(dataCus.totalRecords);
      SetIsLoading(false);
    }
  };
  const fetchData = async (page) => {
    SetIsLoading(true);
    if (selectedId && Object.keys(selectedId).length > 0) {
      const dataCus = await getData(
        `PriceTable/GetListPriceTableByContractId?Id=${selectedId.maHopDong}&PageNumber=${page}&PageSize=${perPage}&onlyct=${onlyCT}`
      );
      setData(dataCus.data);
      setTotalRows(dataCus.totalRecords);
      SetIsLoading(false);
    }
  };

  const handlePageChange = async (page) => {
    await fetchData(page);
  };

  return (
    <div>
      <div className="card card-primary">
        <div className="card-header">
          <h3 className="card-title">{title}</h3>
        </div>
        <div>
          <div className="card-body">
            <div className="container-datatable" style={{ height: "50vm" }}>
              <DataTable
                columns={columns}
                data={data}
                progressPending={IsLoading}
                pagination
                paginationServer
                paginationTotalRows={totalRows}
                onChangeRowsPerPage={handlePerRowsChange}
                onChangePage={handlePageChange}
                highlightOnHover
                striped
                direction="auto"
                responsive
              />
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default PriceListContract;
