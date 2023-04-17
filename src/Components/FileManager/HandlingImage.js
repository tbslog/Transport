import { useMemo, useState, useEffect, useCallback, useRef } from "react";
import { getData, postData, getFileImage } from "../Common/FuncAxios";
import DataTable from "react-data-table-component";
import moment from "moment";
import Lightbox from "react-awesome-lightbox";
import "react-awesome-lightbox/build/style.css";
import Cookies from "js-cookie";
import ConfirmDialog from "../Common/Dialog/ConfirmDialog";
import CreateDocFile from "./CreateDocFile";
import { Modal } from "bootstrap";
import UpdateDocFile from "./UpdateDocFile";

const HandlingImage = (props) => {
  const { dataClick, checkModal } = props;
  const accountType = Cookies.get("AccType");

  const columns = useMemo(() => [
    {
      name: "Xem Hình",
      cell: (val) => (
        <>
          <button
            onClick={() =>
              handleOnclickShowImage(val, SetShowModal("ShowImage"))
            }
            type="button"
            className="btn btn-title btn-sm btn-default mx-2"
            gloss="Xem Hình Ảnh"
          >
            <i className="far fa-eye"></i>
          </button>
          {accountType && accountType !== "KH" && (
            <>
              <button
                onClick={() =>
                  showModalForm(
                    setSelectIdClick(val),
                    SetShowModal("UpdateDoc"),
                    setTitle("Cập nhật chứng từ")
                  )
                }
                type="button"
                className="btn btn-title btn-sm btn-default mx-2"
                gloss="Cập nhật"
              >
                <i className="far fa-edit"></i>
              </button>
              <button
                onClick={() =>
                  showConfirmDialog(val, setFuncName("removeImage"))
                }
                type="button"
                className="btn btn-title btn-sm btn-default mx-2"
                gloss="Xóa Hình Ảnh"
              >
                <i className="fas fa-trash"></i>
              </button>
            </>
          )}
        </>
      ),
      ignoreRowClick: true,
      allowOverflow: true,
      button: true,
      width: "150px",
    },
    {
      selector: (row) => row.maHinhAnh,
      omit: true,
    },
    {
      name: "Trạng Thái",
      selector: (row) => row.trangThai,
      sortable: true,
    },
    {
      name: "Loại Chứng Từ",
      selector: (row) => row.tenLoaiChungTu,
      sortable: true,
    },

    {
      name: "Người Tạo",
      selector: (row) => row.creator,
      sortable: true,
    },
    {
      name: "Thời Gian Tải Lên",
      selector: (row) => moment(row.createdTime).format("DD/MM/YYYY HH:mm:ss"),
      sortable: true,
    },
  ]);

  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(false);

  const [funcName, setFuncName] = useState("");

  const [ShowModal, SetShowModal] = useState("");
  const [ShowConfirm, setShowConfirm] = useState(false);

  const [selectIdClick, setSelectIdClick] = useState({});
  const [image, setImage] = useState("");
  const [name, setName] = useState("");

  const [title, setTitle] = useState("");
  const [modal, setModal] = useState(null);
  const parseExceptionModal = useRef();

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

  const handleDeleteImage = async () => {
    if (
      ShowConfirm === true &&
      selectIdClick &&
      Object.keys(selectIdClick).length > 0
    ) {
      const fileId = selectIdClick.maChungTu;
      var del = await postData(`BillOfLading/DeleteImage?docId=${fileId}`);

      if (del === 1) {
        await fetchData(dataClick.maDieuPhoi);
        setShowConfirm(false);
      }
    }
  };

  const showConfirmDialog = (val) => {
    if (val) {
      setSelectIdClick(val);
    }
    setShowConfirm(true);
  };

  const funcAgree = async () => {
    if (funcName && funcName.length > 0) {
      switch (funcName) {
        case "removeImage":
          return await handleDeleteImage();
        // case "LockDoc":
        //   return await ChangeStatusDoc("Lock");
        // case "ConfirmDoc":
        //   return await ChangeStatusDoc("Confirm");
      }
    }
  };

  useEffect(() => {
    if (props && dataClick && Object.keys(dataClick).length > 0) {
      if (!dataClick.maDieuPhoi) {
        return;
      }
      fetchData(dataClick.maDieuPhoi);
    }
  }, [props, dataClick, checkModal]);

  const fetchData = async (maDieuPhoi) => {
    setLoading(true);
    const data = await getData(
      `BillOfLading/GetListImage?handlingId=${maDieuPhoi}`
    );

    setData(data);
    setLoading(false);
  };

  const handleEditButtonClick = (value) => {
    setSelectIdClick(value);
  };

  const handleOnclickShowImage = (val) => {
    (async () => {
      var getImage = await getFileImage(
        `BillOfLading/GetImageById?id=${val.maHinhAnh}`
      );
      setName(val.fileName);
      setImage(getImage);
    })();
  };

  return (
    <>
      <section className="content">
        <div className="card">
          <div className="card-header">
            <div className="container-fruid">
              {accountType && accountType !== "KH" && (
                <>
                  <div className="row">
                    <button
                      className="btn btn-title btn-sm btn-default mx-1"
                      gloss="Thêm mới Chứng Từ"
                      type="button"
                      onClick={() =>
                        showModalForm(
                          SetShowModal("CreateDoc"),
                          setTitle("Thêm mới chứng từ")
                        )
                      }
                    >
                      <i className="fas fa-plus-circle"></i>
                    </button>
                    <button
                      className="btn btn-title btn-sm btn-default mx-1"
                      gloss="Xác Nhận Đã Đủ Chứng Từ"
                      type="button"
                      onClick={() =>
                        showConfirmDialog(setFuncName("ConfirmDoc"))
                      }
                    >
                      <i className="far fa-check-circle"></i>
                    </button>
                    <button
                      className="btn btn-title btn-sm btn-default mx-1"
                      gloss="Khóa Chứng Từ"
                      type="button"
                      onClick={() => showConfirmDialog(setFuncName("LockDoc"))}
                    >
                      <i className="fas fa-lock"></i>
                    </button>
                  </div>
                </>
              )}
            </div>
          </div>
          <div className="card-body">
            <div className="container-datatable" style={{ height: "50vm" }}>
              <DataTable
                columns={columns}
                data={data}
                progressPending={loading}
                highlightOnHover
                fixedHeader
                fixedHeaderScrollHeight="60vh"
              />
            </div>
          </div>
          <div className="card-footer"></div>
        </div>

        {ShowModal === "ShowImage" && (
          <div>
            <Lightbox
              images={[{ url: image, title: name }, {}]}
              onClose={() => SetShowModal("HideImage")}
            ></Lightbox>
          </div>
        )}
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
      </section>
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
              <h5>{title}</h5>
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
                {ShowModal === "CreateDoc" && (
                  <CreateDocFile
                    selectIdClick={dataClick}
                    refeshData={fetchData}
                  />
                )}
                {ShowModal === "UpdateDoc" && (
                  <UpdateDocFile
                    dataClick={selectIdClick}
                    refeshData={fetchData}
                    hideModal={hideModal}
                  />
                )}
              </>
            </div>
          </div>
        </div>
      </div>
    </>
  );
};

export default HandlingImage;
